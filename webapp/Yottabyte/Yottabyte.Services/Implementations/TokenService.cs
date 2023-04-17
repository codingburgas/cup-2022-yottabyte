using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Yottabyte.Data.Models.Auth;
using Yottabyte.Data;
using Yottabyte.Services.Contracts;
using Yottabyte.Shared.Models.Auth;

namespace Yottabyte.Services.Implementations;

/// <summary>
/// Token Service.
/// </summary>
internal class TokenService : ITokenService
{
    private readonly IConfiguration configuration;
    private readonly UserManager<User> userManager;
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="userManager">User manager.</param>
    /// <param name="context">DB Context.</param>
    public TokenService(IConfiguration configuration, UserManager<User> userManager, ApplicationDbContext context)
    {
        this.configuration = configuration;
        this.userManager = userManager;
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<TokenModel> CreateTokensForUserAsync(string? email)
    {
        var user = await this.userManager.FindByEmailAsync(email);

        var userRoles = await this.userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var accessToken = this.CreateToken(authClaims, TokenTypes.AccessToken);
        var refreshToken = this.CreateToken(authClaims, TokenTypes.RefreshToken);

        await this.SaveRefreshTokenAsync(new RefreshToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(refreshToken),
            UserId = user.Id,
        });

        return new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    /// <inheritdoc/>
    public async Task<string> GenerateEmailConfirmationTokenAsync(string? email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        return await this.userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    /// <inheritdoc/>
    public async Task<TokenModel> CreateNewTokensAsync(TokenInputModule tokenModel)
    {
        var principal = this.GetPrincipalFromExpiredToken(tokenModel.AccessToken);

        if (principal == null)
        {
            return new()
            {
                AccessToken = null,
                RefreshToken = null,
            };
        }

        var user = await this.userManager.FindByIdAsync(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var refreshToken = await this.GetRefreshTokenAsync(tokenModel.RefreshToken);

        if (user == null || refreshToken == null || refreshToken.UserId != user.Id || !this.ValidateRefreshToken(tokenModel.RefreshToken))
        {
            return new()
            {
                AccessToken = null,
                RefreshToken = null,
            };
        }

        await this.DeleteRefreshTokenAsync(refreshToken);

        var newRefreshToken = this.CreateToken(principal.Claims.ToList(), TokenTypes.RefreshToken);

        await this.SaveRefreshTokenAsync(new RefreshToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(newRefreshToken),
            UserId = user.Id,
        });

        return new()
        {
            AccessToken = this.CreateToken(principal.Claims.ToList(), TokenTypes.AccessToken),
            RefreshToken = newRefreshToken,
        };
    }

    // issuer: _configuration["JWT:ValidIssuer"],
    // audience: _configuration["JWT:ValidAudience"],

    /// <inheritdoc/>
    public async Task<string> GeneratePasswordResetTokenAsync(string? email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        return await this.userManager.GeneratePasswordResetTokenAsync(user);
    }

    /// <inheritdoc/>
    public IEnumerable<Claim> ExtractClaims(string jwtToken)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(jwtToken);
        IEnumerable<Claim> claims = securityToken.Claims;
        return claims;
    }

    /// <inheritdoc/>
    public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
    {
        this.context.Add(refreshToken);

        await this.context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<RefreshToken?> GetRefreshTokenAsync(string? token)
    {
        return this.context.RefreshTokens?.FirstOrDefault(rt => rt.Token == token);
    }

    /// <inheritdoc/>
    public async Task DeleteRefreshTokenAsync(RefreshToken refreshToken)
    {
        this.context.RefreshTokens?.Remove(refreshToken);

        await this.context.SaveChangesAsync();
    }

    private bool ValidateRefreshToken(string? token)
    {
        var tokenValidationParameter = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:RefreshTokenSecret"])),
            ValidateLifetime = false,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        _ = tokenHandler.ValidateToken(token, tokenValidationParameter, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameter = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:Secret"])),
            ValidateLifetime = false,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameter, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    /// <summary>
    /// Create token.
    /// </summary>
    /// <param name="authClaims">Authentication claims.</param>
    /// <param name="tokenType">Token type.</param>
    /// <returns>JWT Security Token.</returns>
    private JwtSecurityToken CreateToken(List<Claim> authClaims, TokenTypes tokenType)
    {
        SymmetricSecurityKey? authSigningKey;

        int tokenValidity = 0;

        if (tokenType == TokenTypes.AccessToken)
        {
            authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:Secret"]));
            _ = int.TryParse(this.configuration["JWT:AccessTokenValidityInMinutes"], out tokenValidity);
        }
        else
        {
            authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:RefreshTokenSecret"]));
            _ = int.TryParse(this.configuration["JWT:RefreshTokenValidityInDays"], out tokenValidity);
        }

        var token = new JwtSecurityToken(
            expires: tokenType == TokenTypes.AccessToken ? DateTime.Now.AddMinutes(tokenValidity) : DateTime.Now.AddDays(tokenValidity),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }
}