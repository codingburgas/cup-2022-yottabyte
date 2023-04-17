using System.IdentityModel.Tokens.Jwt;

namespace Yottabyte.Shared.Models.Auth;

/// <summary>
/// Tokens for the users.
/// </summary>
public class TokenModel
{
    /// <summary>
    /// Gets or sets access token.
    /// </summary>
    public JwtSecurityToken? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets refresh token.
    /// </summary>
    public JwtSecurityToken? RefreshToken { get; set; }
}