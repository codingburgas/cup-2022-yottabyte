using System.ComponentModel.DataAnnotations;

namespace Yottabyte.Shared.Models.Auth;

/// <summary>
/// Model for registering a password.
/// </summary>
public class ResetPasswordModel
{
    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the token for reseting a password.
    /// </summary>
    public string? Token { get; set; }
}
