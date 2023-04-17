using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Yottabyte.Shared.Models;
using Yottabyte.Shared.Models.Auth;
using Yottabyte.Services.Contracts;
using System.Linq;

namespace Yottabyte.Server.Controllers.User;

[Route("api/[controller]")]
[ApiController]
public class UserAuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly ITokenService tokenService;
    private readonly IEmailService emailService;
    private readonly ICurrentUser currentUser;
    private readonly IConfiguration configuration;
    private readonly ILogger<UserAuthController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAuthController"/> class.
    /// </summary>
    /// <param name="authService">Authentication Service.</param>
    /// <param name="tokenService">Token Service.</param>
    /// <param name="emailService">Email Service.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="currentUser">Current User.</param>
    /// <param name="logger">Logger.</param>
    public UserAuthController(
        IAuthService authService,
        ITokenService tokenService,
        IEmailService emailService,
        ICurrentUser currentUser,
        IConfiguration configuration,
        ILogger<UserAuthController> logger)
    {
        this.authService = authService;
        this.tokenService = tokenService;
        this.emailService = emailService;
        this.currentUser = currentUser;
        this.configuration = configuration;
        this.logger = logger;
    }


    /// <summary>
    /// Route to login a user.
    /// </summary>
    /// <param name="model">Email and Password.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginAsync([FromForm] LoginModel model)
    {
        this.logger.LogInformation("User with email {Email} is trying to login.", model.Email);

        if (!await this.authService.CheckIfUserExistsAsync(model.Email))
        {
            this.logger.LogWarning("User with email {Email} does not exist.", model.Email);
            return this.BadRequest(new Response { Status = "login-failed", Message = "Invalid Email." });
        }

        if (!await this.authService.CheckIfUserHasVerifiedEmailAsync(model.Email))
        {
            this.logger.LogWarning("User with email {Email} has not verified their email.", model.Email);
            return this.BadRequest(new Response { Status = "login-fielded", Message = "Email isn't verified." });
        }

        if (!await this.authService.CheckIsPasswordCorrectAsync(model.Email, model.Password))
        {
            this.logger.LogWarning("User with email {Email} has entered an incorrect password.", model.Email);
            return this.BadRequest(new Response { Status = "login-failed", Message = "Invalid Password." });
        }

        TokenModel tokens = await this.tokenService.CreateTokensForUserAsync(model.Email);

        this.logger.LogInformation("User with email {Email} has logged in successfully.", model.Email);

        return this.Ok(new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(tokens.AccessToken),
            RefreshToken = new JwtSecurityTokenHandler().WriteToken(tokens.RefreshToken),
            Expiration = tokens.AccessToken!.ValidTo,
        });
    }

    /// <summary>
    /// Register a user.
    /// </summary>
    /// <param name="model">Register model.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync([FromForm] UserIM model)
    {
        this.logger.LogInformation("User with email {Email} is trying to register.", model.Email);

        if (await this.authService.CheckIfUserExistsAsync(model.Email))
        {
            this.logger.LogWarning("User with email {Email} already exists.", model.Email);
            return this.Conflict(new Response { Status = "register-failed", Message = "User already exists." });
        }

        var response = await this.authService.CreateUserAsync(model);

        if (!response.Item1)
        {
            this.logger.LogWarning("User with email {Email} failed to register. Reason: {Reason}", model.Email, response.Item2);
            return this.BadRequest(new Response { Status = "register-failed", Message = response.Item2 });
        }

        var token = await this.tokenService.GenerateEmailConfirmationTokenAsync(model.Email);
        var callbackUrl = this.Url.Action("ConfirmEmail", "UserAuth", new { email = model.Email, token }, this.Request.Scheme, this.Request.Host.Value);

        await this.emailService.SendEmailAsync(new(
            model.Email!,
            "Confirm your email",
            $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>."));

        this.logger.LogInformation("User with email {Email} has registered successfully.", model.Email);

        return this.Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    /// <summary>
    /// Renews a access token.
    /// </summary>
    /// <param name="tokenModel">Token model.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [Route("renew")]
    public async Task<IActionResult> RenewAsync([FromForm] TokenInputModule tokenModel)
    {
        if (tokenModel is null)
        {
            return this.Unauthorized(new Response { Status = "renew-failed", Message = "No token provided." });
        }

        var tokens = await this.tokenService.CreateNewTokensAsync(tokenModel);

        if (tokens.AccessToken is null)
        {
            return this.BadRequest(new Response { Status = "renew-failed", Message = "Invalid token." });
        }

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(tokens.AccessToken),
            refreshToken = new JwtSecurityTokenHandler().WriteToken(tokens.RefreshToken),
            Expiration = tokens.AccessToken!.ValidTo,
        });
    }

    /// <summary>
    /// Confirms email of a user.
    /// </summary>
    /// <param name="token">Token.</param>
    /// <param name="email">Email.</param>
    /// <returns>Response with the result.</returns>
    [HttpGet]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync(string token, string email)
    {
        this.logger.LogInformation("User with email {Email} is trying to confirm their email.", email);

        if (!await this.authService.CheckIfUserExistsAsync(email))
        {
            this.logger.LogWarning("User with email {Email} does not exist.", email);
            return this.BadRequest(new Response { Status = "confirm-email-failed", Message = "User does not exist." });
        }

        var result = await this.authService.ConfirmEmailAsyncAsync(email, token);

        if (!result.Succeeded)
        {
            this.logger.LogWarning("User with email {Email} failed to confirm their email. Reason: {Reason}", email, result.Errors.FirstOrDefault().Description);
            return this.BadRequest(result.Errors.Select(e => e.Description).ToArray());
        }

        this.logger.LogInformation("User with email {Email} has confirmed their email successfully.", email);

        return this.Ok(new Response { Status = "Success", Message = "Email confirmed successfully!" });
    }

    /// <summary>
    /// Sends email for resetting a password.
    /// </summary>
    /// <param name="model">Model for forgotten password.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync([FromForm] ForgotPasswordModel model)
    {
        this.logger.LogInformation("User with email {Email} is trying to send email to reset their password.", model.Email);

        if (!await this.authService.CheckIfUserExistsAsync(model.Email))
        {
            this.logger.LogWarning("User with email {Email} does not exist.", model.Email);
            return this.BadRequest(new Response { Status = "forgot-password-failed", Message = "User does not exist." });
        }

        var token = await this.tokenService.GeneratePasswordResetTokenAsync(model.Email);
        var callbackUrl = this.Url.Action(nameof(this.ResetPasswordAsync), "UserAuth", new { email = model.Email, token }, this.Request.Scheme);

        await this.emailService.SendEmailAsync(new(
            model.Email!,
            "Reset your password",

            $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>." /*,
           $"Here is your reset password token: {token}"*/));

        this.logger.LogInformation("User with email {Email} has successfully send a password recovery email.", model.Email);

        return this.Ok(new Response { Status = "Success", Message = "Password reset email sent successfully!" });
    }

    /// <summary>
    /// Resets the user password.
    /// </summary>
    /// <param name="model">Model for resetting a password.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordModel model)
    {
        this.logger.LogInformation("User with email {Email} is trying to reset their password.", model.Email);

        if (!await this.authService.CheckIfUserExistsAsync(model.Email))
        {
            this.logger.LogWarning("User with email {Email} does not exist.", model.Email);
            return this.BadRequest(new Response { Status = "reset-password-failed", Message = "User does not exist." });
        }

        var result = await this.authService.ResetPasswordAsync(model.Email!, model.Token!, model.Password!);

        if (!result.Succeeded)
        {
            this.logger.LogWarning("User with email {Email} failed to reset their password. Reason: {Reason}", model.Email, result.Errors.FirstOrDefault().Description);
            return this.BadRequest(result.Errors.Select(e => e.Description).ToArray());
        }

        this.logger.LogInformation("User with email {Email} has succesfully reset their password.", model.Email);

        return this.Ok(new Response { Status = "Success", Message = "Password reset successfully!" });
    }
}
