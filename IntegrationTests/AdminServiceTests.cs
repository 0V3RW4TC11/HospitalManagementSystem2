using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TestData;

namespace IntegrationTests;

internal class AdminServiceTests : IntegrationTestBase
{
    public AdminServiceTests()
    {
        SeedAdminRole();
    }

    private void SeedAdminRole()
    {
        var roleManager = DbHelper.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        roleManager.CreateAsync(new IdentityRole(AuthRoles.Admin)).GetAwaiter().GetResult();
    }
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateAdmin_AdminWithValidData_CreatesAdminAndAccount()
    {
        // Act
        await GetServiceManager().AdminService.CreateAsync(AdminData.CreateDto);
        
        // Assert
        Admin? admin = null;
        Account? account = null;
        IdentityUser? identityUser = null;
        
        // Has unique Admin record
        Assert.DoesNotThrow(() =>
        {
            admin = GetDbContext().Admins.Single(a => 
                a.FirstName == AdminData.CreateDto.FirstName &&
                a.LastName == AdminData.CreateDto.LastName &&
                a.Gender == AdminData.CreateDto.Gender &&
                a.Address == AdminData.CreateDto.Address &&
                a.Phone == AdminData.CreateDto.Phone &&
                a.Email == AdminData.CreateDto.Email &&
                a.DateOfBirth == AdminData.CreateDto.DateOfBirth);
        });
        
        // Has unique Account record
        Assert.DoesNotThrow(() =>
        {
            account = GetDbContext().Accounts.Single(a => a.UserId == admin!.Id);
        });
        
        // Has unique Identity record
        Assert.DoesNotThrow(() =>
        {
            identityUser = GetDbContext().Users.Single(a => a.Id == account!.IdentityUserId);
        });
        
        // Has expected username
        Assert.That(identityUser!.UserName, Is.EqualTo(AdminData.ExpectedUsername));
    }

    [Test]
    public async Task CreateAsync_AdminWithInvalidData_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task CreateAsync_DuplicateAdmin_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingAdmin_ReturnsAdmin()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task GetByIdAsync_NonExistingAdmin_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task UpdateAsync_ExistingAdminWithValidData_UpdatesAdmin()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task UpdateAsync_ExistingAdminWithInvalidData_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task UpdateAsync_NonExistingAdmin_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task DeleteAsync_ExistingAdmin_DeletesAdminAndAccount()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task DeleteAsync_NonExistingAdmin_Throws()
    {
        throw new NotImplementedException();
    }
}