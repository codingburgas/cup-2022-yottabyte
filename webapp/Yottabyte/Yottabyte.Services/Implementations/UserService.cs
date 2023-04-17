using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Yottabyte.Data.Models.Auth;
using Yottabyte.Services.Contracts;
using Yottabyte.Shared.Models.Auth;

namespace Yottabyte.Services.Implementations;

/// <summary>
/// User service.
/// </summary>
internal class UserService : IUserService
{
    private readonly UserManager<User> userManager;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="userManager">User Manager.</param>
    /// <param name="mapper">AutoMapper.</param>
    public UserService(UserManager<User> userManager, IMapper mapper)
    {
        this.userManager = userManager;
        this.mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<UserVM?> GetUserByIdAsync(string id)
    {
        var user = await this.userManager.FindByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        return this.mapper.Map<UserVM>(user);
    }

    /// <inheritdoc/>
    public List<UserVM> GetAllUsers()
    {
        List<UserVM> UserVM = new();

        this.userManager.Users.ToList().ForEach(
            async user =>
            {
                UserVM.Add(this.mapper.Map<UserVM>(user));
            });

        return UserVM;
    }

    /// <inheritdoc/>
    public async Task<string> GetUserEmailByIdAsync(string id)
    {
        var user = await this.userManager.FindByIdAsync(id);

        return await this.userManager.GetEmailAsync(user);
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> ChangePasswordAsync(string userId, string newPassword)
    {
        var user = await this.userManager.FindByIdAsync(userId);

        var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

        return await this.userManager.ResetPasswordAsync(user, token, newPassword);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateUserAsync(UserVM oldUserInfo, UserIM newUserInfo)
    {
        var user = await this.userManager.FindByEmailAsync(oldUserInfo.Email);

        if (newUserInfo.Password is not null)
        {
            var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

            var result = await this.userManager.ResetPasswordAsync(user, token, newUserInfo.Password);

            if (!result.Succeeded)
            {
                return false;
            }
        }

        user.FirstName = newUserInfo.FirstName;
        user.LastName = newUserInfo.LastName;
        user.Email = newUserInfo.Email;
        user.UserName = newUserInfo.Email;

        await this.userManager.UpdateAsync(user);
        return true;
    }

    /// <inheritdoc/>
    public async Task<string> GenerateOneTimeTokenForUserAsync(string email, string type, string purpose)
    {
        var user = await this.userManager.FindByEmailAsync(email);

        return await this.userManager.GenerateUserTokenAsync(user, type, purpose);
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateOneTimeTokenForUserAsync(string userId, string token, string type, string purpose)
    {
        var user = await this.userManager.FindByIdAsync(userId);

        return await this.userManager.VerifyUserTokenAsync(user, type, purpose, token);
    }

    /// <inheritdoc/>
    public async Task<UserVM> GetUserByEmailAsync(string email)
    {
        return this.mapper.Map<UserVM>(await this.userManager.FindByEmailAsync(email));
    }
}