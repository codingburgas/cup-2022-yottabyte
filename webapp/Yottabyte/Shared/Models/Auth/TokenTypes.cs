using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Shared.Models.Auth;

/// <summary>
/// Token types.
/// </summary>
public enum TokenTypes
{
    /// <summary>
    /// Access Token
    /// </summary>
    AccessToken,

    /// <summary>
    /// Refresh Token
    /// </summary>
    RefreshToken,
}