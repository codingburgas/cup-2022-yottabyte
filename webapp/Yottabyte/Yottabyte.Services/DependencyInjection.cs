using Microsoft.Extensions.DependencyInjection;
using Yottabyte.Services.Contracts;
using Yottabyte.Services.Implementations;

namespace Yottabyte.Services;

/// <summary>
/// Dependency Injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add Services.
    /// </summary>
    /// <param name="services">Services.</param>
    public static void AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IEmailService, EmailService>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IFileService, FileService>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<ICustomVisionService, CustomVisionService>()
            .AddScoped<IGeolocationService, GeolocationService>()
            .AddScoped<ITimeZoneService, TimeZoneService>()
            .AddScoped<IDateService, DateService>();
    }
}