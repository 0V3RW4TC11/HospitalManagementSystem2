using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class IdentityProviderOld : IIdentityProviderOld
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IdentityProviderOld(
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public async Task<string> CreateAsync(string username, string password)
    {
        var user = new IdentityUser { UserName = username };
        var result = await _userManager.CreateAsync(user, password);
        IdentityResultThrowOnFail(result);
        
        return user.Id;
    }

    public async Task RemoveAsync(string identityId)
    {
        var user = await GetIdentityAsync(identityId);
        var result = await _userManager.DeleteAsync(user);
        IdentityResultThrowOnFail(result);
    }

    public async Task CreateRoleAsync(string roleName)
    {
        var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
        IdentityResultThrowOnFail(result);
    }

    public async Task RemoveRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName)
            ?? throw new Exception("Role '" + roleName + "' not found");
        var result = await _roleManager.DeleteAsync(role);
        IdentityResultThrowOnFail(result);
    }

    public async Task AddToRoleAsync(string identityId, string role)
    {
        var user = await GetIdentityAsync(identityId);
        var result = await _userManager.AddToRoleAsync(user, role);
        IdentityResultThrowOnFail(result);
    }

    public async Task LoginAsync(
        string username, 
        string password, 
        bool isPersistent, 
        bool enableLockoutOnFail)
    {
        var result = await _signInManager.PasswordSignInAsync(
            username,
            password,
            isPersistent,
            lockoutOnFailure: enableLockoutOnFail);

        if (result.Succeeded is false)
            throw new Exception(result.ToString());
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task ChangePasswordAsync(string identityId, string oldPassword, string newPassword)
    {
        var identity = await GetIdentityAsync(identityId);
        var result = await _userManager.ChangePasswordAsync(identity, oldPassword, newPassword);
        IdentityResultThrowOnFail(result);
    }

    public async Task ResetPasswordAsync(string identityId, string newPassword)
    {
        var identity = await GetIdentityAsync(identityId);
        var token = await _userManager.GeneratePasswordResetTokenAsync(identity);
        var result = await _userManager.ResetPasswordAsync(identity, token, newPassword);
        IdentityResultThrowOnFail(result);
    }

    public async Task<string> GetUserNameAsync(string identityId)
    {
        var identity = await GetIdentityAsync(identityId);
        return identity.UserName
            ?? throw new Exception("Username not found for Identity Id: " + identityId);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> IsLockedOut(string identityId)
    {
        var identity = await GetIdentityAsync(identityId);
        return await _userManager.IsLockedOutAsync(identity);
    }

    public async Task SetLockoutAsync(string identityId, bool enabled)
    {
        var identity = await GetIdentityAsync(identityId);
        
        identity.LockoutEnabled = enabled;
        identity.LockoutEnd = enabled ? DateTimeOffset.MaxValue : null;

        var result = await _userManager.UpdateAsync(identity);
        IdentityResultThrowOnFail(result);
    }

    public string GetLoggedInUserIdentityId()
    {
        var user = _signInManager.Context.User;

        if (_signInManager.IsSignedIn(user))
        {
            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("User ID not found in claims.");

            return userId;
        }

        throw new Exception("No user is currently signed in.");
    }

    private async Task<IdentityUser> GetIdentityAsync(string identityId)
    {
        var user = await _userManager.FindByIdAsync(identityId)
            ?? throw new Exception($"Identity not found for Identity Id {identityId}");

        return user;
    }

    private static void IdentityResultThrowOnFail(IdentityResult result)
    {
        if (result.Succeeded is false)
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
    }
}