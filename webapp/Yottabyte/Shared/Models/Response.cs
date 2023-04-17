using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Shared.Models;

/// <summary>
/// An API Response.
/// </summary>
public class Response
{
    /// <summary>
    /// Gets or sets the status of the response.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the message of the response.
    /// </summary>
    public string? Message { get; set; }
}
