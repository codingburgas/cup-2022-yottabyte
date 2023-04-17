using System.ComponentModel.DataAnnotations.Schema;

namespace Yottabyte.Data.Models.Auth;

/// <summary>
/// Refresh token model.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets id of the refresh token.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets refresh token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets id of the user to which the refresh token belongs.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets user to which the refresh token belongs.
    /// </summary>
    [ForeignKey("UserId")]
    public User? User { get; set; }
}