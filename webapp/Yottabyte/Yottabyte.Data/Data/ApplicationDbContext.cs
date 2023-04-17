using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yottabyte.Data.Models.Auth;
using Yottabyte.Data.Models.Events;

namespace Yottabyte.Data;

/// <summary>
/// Application database context.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">Options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }


    /// <summary>
    /// Gets or sets RefreshTokens.
    /// </summary>
    public virtual DbSet<RefreshToken>? RefreshTokens { get; set; }

    /// <summary>
    /// Gets or sets Events.
    /// </summary>
    public virtual DbSet<Event>? Events { get; set; }
}
