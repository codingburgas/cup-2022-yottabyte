﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yottabyte.Server.Data;
using Yottabyte.Shared;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Azure.Storage;
using Azure.Storage.Blobs;
using System.IO;
using Azure.Storage.Blobs.Specialized;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Yottabyte.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/users
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<UserVM>>> GetUser()
        {
            List<UserVM> usersVM = new List<UserVM>();

            (await _context.User.ToListAsync()).ForEach(
                u => usersVM.Add(UserToVM(u))
            );

            return usersVM;
        }

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

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromForm] UserIM userIM)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var claims = ExtractClaims(token);

            int reqUserId = Int32.Parse(claims.ToArray()[0].Value);
            bool isReqUserAdmin = claims.ToArray()[1].Value == "True";

            if (reqUserId != id && !isReqUserAdmin)
            {
                return Unauthorized("The user isn't an admin");
            }

            User user = await _context.User
                .FirstOrDefaultAsync(u => u.Id == id);

            user.FName = userIM.FName;
            user.LName = userIM.LName;
            user.Email = userIM.Email;

            if (userIM.Avatar != null)
            {
                string[] permittedExtensions = { ".png", ".jpg", ".gif", ".svg" };

                var ext = Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    return BadRequest("The file extension of avatar image is invalid");
                }

                var connectionString = (string)_configuration["AzureStorage:ConnectionString"];

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

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

                blockBlobClient = containerClient.GetBlockBlobClient(Path.GetRandomFileName() + Guid.NewGuid().ToString() + Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant());
                await blockBlobClient.UploadAsync(userIM.Avatar.OpenReadStream());

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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("user-update-succsess");
        }

        // POST: api/users/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> RegisterUser([FromForm] UserIM userIM)
        {
            // Convert the input model to normal model
            User user = IMToUser(userIM);

            // Check for duplicate email
            bool doesUserExist = await _context.User
                .FirstOrDefaultAsync(u => u.Email == user.Email) != null;

            if (doesUserExist)
            {
                return Conflict("There is already a user with this email!");
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
                    return BadRequest("The file extension of avatar image is invalid");
                }

                var connectionString = (string)_configuration["AzureStorage:ConnectionString"];

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                string containerName = "yottabyteavatarimagestest";

                BlobContainerClient containerClient;

                containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                containerClient.CreateIfNotExists();

                BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(Path.GetRandomFileName() + Guid.NewGuid().ToString() + Path.GetExtension(userIM.Avatar.FileName).ToLowerInvariant());
                await blockBlobClient.UploadAsync(userIM.Avatar.OpenReadStream());

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

            return Ok("user-register-success");
        }

        // POST: api/users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> LoginUser([FromForm] string Email, [FromForm] string Password)
        {
            User user = await _context.User
                .FirstOrDefaultAsync(u => u.Email == Email);

            bool doesUserExist = user != null;

            if (!doesUserExist)
            {
                return BadRequest("There isn't a user with this email!");
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

                return Ok(tokenHandler.WriteToken(token));
            }
            else
            {
                return BadRequest("The password is incorrect!");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var claims = ExtractClaims(token);

            int reqUserId = Int32.Parse(claims.ToArray()[0].Value);
            bool isReqUserAdmin = claims.ToArray()[1].Value == "True";

            if (reqUserId != id && !isReqUserAdmin)
            {
                return Unauthorized("The user isn't an admin");
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound("There isn't a user with this id");
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

            return Ok("user-deletion-success");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private static User IMToUser(UserIM userIM) =>
            new()
            {
                FName = userIM.FName,
                LName = userIM.LName,
                Email = userIM.Email,
                PasswordHash = userIM.Password
            };

        private static UserVM UserToVM(User user) =>
            new()
            {
                Id = user.Id,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                AvatarURL = user.AvatarURL
            };

        public IEnumerable<Claim> ExtractClaims(string jwtToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(jwtToken);
            IEnumerable<Claim> claims = securityToken.Claims;
            return claims;
        }
    }
}
