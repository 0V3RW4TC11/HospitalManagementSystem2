using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class IdentityProvider : IIdentityProvider
{
    private readonly UserManager<IdentityUser> _userManager;
    
    public IdentityProvider(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
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
        var user = await GetUserAsync(identityId);
        var result = await _userManager.DeleteAsync(user);
        
        IdentityResultThrowOnFail(result);
    }

    public async Task AddToRoleAsync(string identityId, string role)
    {
        var user = await GetUserAsync(identityId);
        var result = await _userManager.AddToRoleAsync(user, role);
        
        IdentityResultThrowOnFail(result);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.Email == email);
    }

    private async Task<IdentityUser> GetUserAsync(string identityId)
    {
        var user = await _userManager.FindByIdAsync(identityId)
            ?? throw new Exception($"User not found for Id {identityId}");

        return user;
    }

    private static void IdentityResultThrowOnFail(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
    }
}