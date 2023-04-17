using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yottabyte.Data.Models.Auth;
using Yottabyte.Services.Contracts;
using Yottabyte.Shared.Models.Auth;

namespace Yottabyte.Services.Implementations;

/// <summary>
/// Authentication Service.
/// </summary>
internal class AuthService : IAuthService
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IFileService fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userManager">User manager.</param>
    /// <param name="signInManager">SignIn manager.</param>
    /// <param name="roleManager">Role manager.</param>
    /// <param name="fileService">File service</param>
    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        IFileService fileService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.fileService = fileService;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfUserExistsAsync(string? email)
    {
        return await this.userManager.FindByEmailAsync(email) != null;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIsPasswordCorrectAsync(string? email, string? password)
    {
        var user = await this.userManager.FindByEmailAsync(email);

        return !(user == null || !await this.userManager.CheckPasswordAsync(user, password));
    }

    /// <inheritdoc/>
    public async Task<Tuple<bool, string?>> CreateUserAsync(UserIM registerModel, bool isAdmin)
    {
        var avatarImageUrl = string.Empty;
        
        if (registerModel.AvatarImage is null)
        {
            var avatarSeed = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            avatarImageUrl = "https://avatars.dicebear.com/api/identicon/" + avatarSeed + ".svg";
        }
        else
        {
            avatarImageUrl = await fileService.SaveImageAsync(registerModel.AvatarImage, "avatars");
        }
        
        User user = new()
        {
            Email = registerModel.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerModel.Email,
            FirstName = registerModel.FirstName,
            LastName = registerModel.LastName,
            AvatarUrl = avatarImageUrl
        };

        var result = await this.userManager.CreateAsync(user, registerModel.Password);

        if (!result.Succeeded)
        {
            return new(false, result.Errors.FirstOrDefault()?.Description);
        }

        if (isAdmin)
        {
            if (!await this.roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await this.roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (await this.roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await this.userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
        }
        else
        {
            if (!await this.roleManager.RoleExistsAsync(UserRoles.User))
            {
                await this.roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            if (await this.roleManager.RoleExistsAsync(UserRoles.User))
            {
                await this.userManager.AddToRoleAsync(user, UserRoles.User);
            }
        }

        return new(true, null);
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> ConfirmEmailAsyncAsync(string email, string token)
    {
        var user = await this.userManager.FindByEmailAsync(email);

        return await this.userManager.ConfirmEmailAsync(user, token);
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string password)
    {
        var user = await this.userManager.FindByEmailAsync(email);

        return await this.userManager.ResetPasswordAsync(user, token, password);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIsAdminAsync(string email)
    {
        var user = await this.userManager.FindByEmailAsync(email);

        var roleList = await this.userManager.GetRolesAsync(user);

        return roleList.Contains(UserRoles.Admin);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfUserHasVerifiedEmailAsync(string? email)
    {
        var user = await this.userManager.FindByEmailAsync(email);

        return !(user == null || !await this.signInManager.CanSignInAsync(user));
    }
}