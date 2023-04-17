using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Shared.Models.Auth;

/// <summary>
/// Model for registering a new user.
/// </summary>
public class ForgotPasswordModel
{
    /// <summary>
    ///   Gets or sets the email address of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}