using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;
using System;
using Yottabyte.Data.Models.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Yottabyte.Server.Helpers;

public static class AuthConfiguration
{
    public static async Task InitAuthSystem(this IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        foreach (var roleName in typeof(UserRoles).GetFields().ToList().Select(p => p.Name).ToList())
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var user = await userManager.FindByEmailAsync(configuration["AdminUser:Email"]);

        if (user is null)
        {
            var avatarSeed = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            var avatarImageUrl = "https://avatars.dicebear.com/api/identicon/" + avatarSeed + ".svg";
            
            var admin = new User
            {
                FirstName = configuration["AdminUser:FirstName"],
                LastName = configuration["AdminUser:LastName"],
                Email = configuration["AdminUser:Email"],
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = configuration["AdminUser:Email"],
                AvatarUrl = avatarImageUrl
            };

            var isSuccessful = await userManager.CreateAsync(admin, configuration["AdminUser:Password"]);

            if (isSuccessful.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, UserRoles.Admin);
            }
        }
    }
}