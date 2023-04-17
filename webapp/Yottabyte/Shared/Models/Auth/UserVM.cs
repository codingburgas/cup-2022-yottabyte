using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Shared.Models.Auth;

/// <summary>
/// View model for the user.
/// </summary>
public class UserVM
{
    /// <summary>
    /// Gets or sets the id of the user.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the role of the user.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the url of the avatar of the user.
    /// </summary>
    public string? AvatarUrl { get; set; }
}