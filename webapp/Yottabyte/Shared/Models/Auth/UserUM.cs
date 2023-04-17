using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Yottabyte.Shared.DataAnnotations;

namespace Yottabyte.Shared.Models.Auth;

/// <summary>
/// Update model for the user.
/// </summary>
public class UserUM
{
    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    [Required]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    [Required]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    [Required]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the image of the user
    /// </summary>
    [Image]
    public IFormFile? AvatarImage { get; set; } 
}
