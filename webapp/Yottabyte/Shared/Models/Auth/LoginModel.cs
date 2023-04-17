using System.ComponentModel.DataAnnotations;

namespace Yottabyte.Shared.Models.Auth;

/// <summary>
/// Model for logging a user.
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}