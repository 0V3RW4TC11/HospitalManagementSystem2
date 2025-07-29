using DataTransfer.Admin;
using Domain.Constants;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace TestData;

public static class AdminTestData
{
    private const string FirstName = "TestAdminFirstName";
    private const string LastName = "TestAdminLastName";
    public static readonly string ExpectedUsername =
        $"{FirstName.ToLower()}.{LastName.ToLower()}@{DomainNames.Organization}";

    public static AdminCreateDto CreateDto() => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Gender = "Male",
        Address = "123 Main St",
        Phone = "123-456-7890",
        Email = "testAdmin@example.com",
        DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
        Password = "Password123!"
    };

    public static async Task<AdminDto> SeedAdmin(RepositoryDbContext context, UserManager<IdentityUser> userManager)
    {
        var adminCreateDto = CreateDto();
        var admin = adminCreateDto.Adapt<Admin>();
        
        context.Admins.Add(admin);
        await context.SaveChangesAsync();
        
        var identity = new IdentityUser {UserName = ExpectedUsername};
        await userManager.CreateAsync(identity, adminCreateDto.Password);
        await userManager.AddToRoleAsync(identity, AuthRoles.Admin);
        
        context.Accounts.Add(new Account {UserId = admin.Id, IdentityUserId = identity.Id});
        await context.SaveChangesAsync();
        
        return admin.Adapt<AdminDto>();
    }
}