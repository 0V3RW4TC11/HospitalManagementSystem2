using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalManagementSystem2.Tests.Helpers;

public static class AccountTestHelper
{
    public static void AssertHasAccount(ApplicationDbContext context, Guid userId, string roleId)
    {
        var account = context.Accounts.SingleOrDefault(a => a.UserId == userId);
        Assert.NotNull(account);
        Assert.Contains(context.Users, u => u.Id == account.IdentityUserId);
        Assert.Contains(context.UserRoles, ur 
            => ur.UserId == account.IdentityUserId && 
               ur.RoleId == roleId);
    }
    
    public static void AssertHasNoAccount(ApplicationDbContext context, Account account)
    {
        Assert.DoesNotContain(context.Accounts, a => a.UserId == account.UserId);;
        Assert.DoesNotContain(context.Users, u => u.Id == account.IdentityUserId);
        Assert.DoesNotContain(context.UserRoles, ur => ur.UserId == account.IdentityUserId);
    }

    public static Account GetAccount(ApplicationDbContext context, Guid userId)
    {
        return context.Accounts.Single(a => a.UserId == userId);
    }

    public static void SeedAccount(ApplicationDbContext context, 
        IServiceProvider provider, 
        Guid userId,
        string username,
        string password,
        string role)
    {
        // Create IdentityUser
        var userMan = provider.GetRequiredService<UserManager<IdentityUser>>();
        var identityUser = new IdentityUser { UserName = username };
        userMan.CreateAsync(identityUser, password).GetAwaiter().GetResult();

        // Add IdentityUser to role
        userMan.AddToRoleAsync(identityUser, role).GetAwaiter().GetResult();
        
        // Create Account for Doctor and IdentityUser
        context.Accounts.Add(new Account
        {
            UserId = userId,
            IdentityUserId = identityUser.Id
        });
        context.SaveChanges();
    }
}