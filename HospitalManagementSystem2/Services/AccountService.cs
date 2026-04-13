using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Services;

public class AccountService
{
    private readonly IDbContext _context;
    private readonly IAccountRepository _accountRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountService(IDbContext context,
        IAccountRepository accountRepository,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _accountRepository = accountRepository;
        _userManager = userManager;
    }

    public async Task CreateAsync(Guid userId, string role, string username, string password)
    {
        var identityUser = new IdentityUser { UserName = username };

        // Create the User
        var result = await _userManager.CreateAsync(identityUser, password);
        CheckIdentityResult(result);

        // Set User's role
        result = await _userManager.AddToRoleAsync(identityUser, role);
        CheckIdentityResult(result);

        // Create Account
        await _accountRepository.AddAsync(new Account { UserId = userId, IdentityUserId = identityUser.Id });
            
        // Save changes to DbContext
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> GetUserIdByIdentityIdAsync(string identityId)
    {
        return await _accountRepository.Accounts
            .Where(a => a.IdentityUserId == identityId)
            .Select(a => a.UserId)
            .FirstOrDefaultAsync();
    }
    
    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var account = await _accountRepository.Accounts.FirstOrDefaultAsync(a => a.UserId == userId)
            ?? throw new Exception($"No account for user Id {userId.ToString()}");
        
        // Delete the Account
        await _accountRepository.RemoveAsync(account);
        
        // Save changes to DbContext
        await _context.SaveChangesAsync();
        
        // Get the IdentityUser
        var identityUser = await _userManager.FindByIdAsync(account.IdentityUserId);
        
        // Delete the IdentityUser
        if (identityUser != null)
        {
            var result = await _userManager.DeleteAsync(identityUser);
            CheckIdentityResult(result);
        }
    }

    public static void CheckIdentityResult(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
    }
}