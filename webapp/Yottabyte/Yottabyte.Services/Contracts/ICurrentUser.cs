using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Services.Contracts;

/// <summary>
/// Interface for current user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the id of the user.
    /// </summary>
    string UserId { get; }
}
