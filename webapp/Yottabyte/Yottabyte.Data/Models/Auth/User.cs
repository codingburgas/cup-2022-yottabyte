using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Yottabyte.Data.Models.Auth;

/// <summary>
/// User model.
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// Gets or sets first name of the user.
    /// </summary>
    [Required]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets last name of the user.
    /// </summary>
    [Required]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the url of the avatar of the user.
    /// </summary>
    [Required]
    public string? AvatarUrl { get; set; }
}