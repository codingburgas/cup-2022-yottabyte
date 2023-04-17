using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Yottabyte.Shared.DataAnnotations;

namespace Yottabyte.Shared.Models.Auth;

public class UserIM
{
    /// <summary>
    /// Gets or sets first name of the user.
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [RegularExpression("^(?=.*[A-ZА-Яа-яa-z])([A-ZА-Я])([a-zа-я]{2,29})+(?<![_.])$")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets last name of the user.
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [RegularExpression("^(?=.*[A-ZА-Яа-яa-z])([A-ZА-Я])([a-zа-я]{2,29})+(?<![_.])$")]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets email of the user.
    /// </summary>
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets password of the user.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the avatar of the user
    /// </summary>
    [Image]
    public IFormFile? AvatarImage { get; set; }
}
