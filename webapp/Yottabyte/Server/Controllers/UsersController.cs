using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Yottabyte.Shared;
using Yottabyte.Server.Data;

namespace Yottabyte.Server.Controllers
{
    /// <summary>
    /// Controler for the user management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// The data contex from the EF
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        ///  The coniguration to get the secrets from the application.json
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Dictionary for converting the file extension to connection type
        /// </summary>
        public readonly Dictionary<string, string> fileExtToConType = new()
        {
             { ".png", "image/x-png" },
             { ".jpg", "image/jpeg" },
             { ".svg", "image/svg+xml" },
            { ".gif", "image/gif" }
        };

        /// <summary>
        /// Constructor for the User Controller
        /// </summary>
        /// <param name="context">Data Context</param>
        /// <param name="configuration">Configuration</param>
        public UsersController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Function to returns all of the users
        /// </summary>
        /// <returns>All of the users</returns>
        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserVM>>> GetUser()
        {
            List<UserVM> usersVM = new();

            (await _context.User.ToListAsync()).ForEach(
                u => usersVM.Add(UserToVM(u))
            );

            return usersVM;
        }

        /// <summary>
        /// Functon to get a user with specific id
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns>The user</returns>
        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserVM>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return UserToVM(user);
        }

        /// <summary>
        /// Funcion for updating the user info
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="userIM">New info for the user</param>
        /// <returns>is there a problem</returns>
        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> PutUser(int id, [FromForm] UserIM userIM)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var claims = ExtractClaims(token);

            int reqUserId = Int32.Parse(claims.ToArray()[0].Value);
            bool isReqUserAdmin = claims.ToArray()[1].Value == "True";

            if (reqUserId != id && !isReqUserAdmin)
            {
                return Unauthorized(new Response { Type = "user-update-failure", Data = "The user isn't an admin" });
            }

            User user = await _context.User
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new Response { Type = "user-update-failure", Data = " Ther is not a user with this id" });
            }

            user.FName = userIM.FName;
            user.LName = userIM.LName;
            user.Email = userIM.Email;

            if (userIM.Avatar != null)
            {
                string[] permittedExtensions = { ".png", ".jpg" };

                var ext = Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    return BadRequest(new Response { Type = "user-update-failure", Data = "The file extension of avatar image is invalid" });
                }

                var connectionString = (string)_configuration["AzureStorage:ConnectionString"];

                BlobServiceClient blobServiceClient = new(connectionString);

                string containerName = "yottabyteavatarimagestest";

                BlobContainerClient containerClient;

                containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                containerClient.CreateIfNotExists();

                BlockBlobClient blockBlobClient;

                if (!user.AvatarURL.StartsWith("https://avatars.dicebear.com/"))
                {
                    // This should be dynamic calculated
                    blockBlobClient = containerClient.GetBlockBlobClient(user.AvatarURL[77..]);
                    await blockBlobClient.DeleteAsync();
                }

                blockBlobClient = containerClient.GetBlockBlobClient(
                    Path.GetRandomFileName() + Guid.NewGuid().ToString() + Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant()
                );

                var blobHttpHeader = new BlobHttpHeaders { ContentType = fileExtToConType[Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant()] };

                await blockBlobClient.UploadAsync(
                    userIM.Avatar.OpenReadStream(),
                    new BlobUploadOptions { HttpHeaders = blobHttpHeader }
                );

                user.AvatarURL = blockBlobClient.Uri.AbsoluteUri;
            }


            // Generate salt
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            // Hash password
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: user.PasswordHash,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));

            user.PasswordHash = hashed;
            user.Salt = Convert.ToBase64String(salt);

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new Response { Type = "user-update-failure", Data = " Ther is not a user with this id" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new Response { Type = "user-update-succsess" });
        }

        /// <summary>
        /// Function for registering of event
        /// </summary>
        /// <param name="userIM">User info</param>
        /// <returns>Was there a problem</returns>
        // POST: api/users/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> RegisterUser([FromForm] UserIM userIM)
        {
            // Convert the input model to normal model
            User user = IMToUser(userIM);

            // Check for duplicate email
            bool doesUserExist = await _context.User
                .FirstOrDefaultAsync(u => u.Email == user.Email) != null;

            if (doesUserExist)
            {
                return Conflict(new Response { Type = "user-register-failure", Data = "There is already a user with this email" });
            }

            user.Role = ROLES.USER;

            // If user doesn't upload avatar
            if (userIM.Avatar == null)
            {
                // Generate custom avatar
                string avatarSeed = (DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
                user.AvatarURL = "https://avatars.dicebear.com/api/identicon/" + avatarSeed + ".svg";
            }
            else
            {
                string[] permittedExtensions = { ".png", ".jpg", ".gif", ".svg" };

                var ext = Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    return BadRequest(new Response { Type = "user-register-failure", Data = "The file extension of avatar image is invalid" });
                }

                var connectionString = (string)_configuration["AzureStorage:ConnectionString"];

                BlobServiceClient blobServiceClient = new(connectionString);

                string containerName = "yottabyteavatarimagestest";

                BlobContainerClient containerClient;

                containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                containerClient.CreateIfNotExists();

                BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(Path.GetRandomFileName() + Guid.NewGuid().ToString() + Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant());

                var blobHttpHeader = new BlobHttpHeaders { ContentType = fileExtToConType[Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant()] };

                await blockBlobClient.UploadAsync(
                    userIM.Avatar.OpenReadStream(),
                    new BlobUploadOptions { HttpHeaders = blobHttpHeader }
                );

                user.AvatarURL = blockBlobClient.Uri.AbsoluteUri;
            }

            // Generate salt
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            // Hash password
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: user.PasswordHash,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));

            user.PasswordHash = hashed;
            user.Salt = Convert.ToBase64String(salt);

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new Response { Type = "user-register-success"});
        }

        /// <summary>
        /// Function for generating JWT Token for the user
        /// </summary>
        /// <param name="Email">Email of the user</param>
        /// <param name="Password">Password of the user</param>
        /// <returns>Was there a problem</returns>
        // POST: api/users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> LoginUser([FromForm] string Email, [FromForm] string Password)
        {
            User user = await _context.User
                .FirstOrDefaultAsync(u => u.Email == Email);

            bool doesUserExist = user != null;

            if (!doesUserExist)
            {
                return BadRequest(new Response { Type = "user-login-failure", Data = "There isn't a user with this email!" });
            }

            // Hash password
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: Convert.FromBase64String(user.Salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));

            if (user.PasswordHash == hashed)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(
                    new Claim[] {
                        new Claim("sub", user.Id.ToString()),
                        new Claim("isAdmin", (user.Role.ToString() == "ADMIN").ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials( new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new Response { Type = "user-login-success", Data = tokenHandler.WriteToken(token) });
            }
            else
            {
                return BadRequest(new Response { Type = "user-login-failure", Data = "The password is incorrect!" });
            }
        }

        /// <summary>
        /// Function for deleting a user
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns>Was there a problem</returns>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteUser(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var claims = ExtractClaims(token);

            int reqUserId = Int32.Parse(claims.ToArray()[0].Value);
            bool isReqUserAdmin = claims.ToArray()[1].Value == "True";

            if (reqUserId != id && !isReqUserAdmin)
            {
                return Unauthorized(new Response { Type = "user-deletion-failure", Data = "The user isn't an admin" });
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new Response { Type = "user-deletion-failure", Data = "There isn't a user with this id" });
            }

            if (!user.AvatarURL.StartsWith("https://avatars.dicebear.com/"))
            {
                var connectionString = (string)_configuration["AzureStorage:ConnectionString"];

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                string containerName = "yottabyteavatarimagestest";

                BlobContainerClient containerClient;

                containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                containerClient.CreateIfNotExists();

                // This should be dynamic calculated
                BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(user.AvatarURL[77..]);
                await blockBlobClient.DeleteAsync();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new Response { Type = "user-deletion-success" });
        }

        /// <summary>
        /// Function for checking if user exists
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns>Does user exists</returns>
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        /// <summary>
        /// Converts user input model to user model
        /// </summary>
        /// <param name="userIM">User Input Model</param>
        /// <returns>User</returns>
        private static User IMToUser(UserIM userIM) =>
            new()
            {
                FName = userIM.FName,
                LName = userIM.LName,
                Email = userIM.Email,
                PasswordHash = userIM.Password
            };

        /// <summary>
        /// Converts user model to user view model
        /// </summary>
        /// <param name="user"></param>
        /// <returns>User View Model</returns>
        private static UserVM UserToVM(User user) =>
            new()
            {
                Id = user.Id,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                AvatarURL = user.AvatarURL
            };

        /// <summary>
        /// Function for extracting the claims from JWT Token
        /// </summary>
        /// <param name="jwtToken">The JWT Token</param>
        /// <returns>The claims</returns>
        public IEnumerable<Claim> ExtractClaims(string jwtToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(jwtToken);
            IEnumerable<Claim> claims = securityToken.Claims;
            return claims;
        }
    }
}
