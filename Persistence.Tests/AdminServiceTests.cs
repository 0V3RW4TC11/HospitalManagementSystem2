using DataTransfer.Admin;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Configurations;
using Persistence.Repositories;
using Services;
using Services.Abstractions;
using TestData;

namespace Persistence.Tests;

internal sealed class AdminServiceTests : PersistenceTestBase
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        SeedAdminRole();
    }

    private void SeedAdminRole()
    {
        var roleManager = GetServiceProvider().GetRequiredService<RoleManager<IdentityRole>>();
        roleManager.CreateAsync(new IdentityRole(AuthRoles.Admin)).GetAwaiter().GetResult();
    }
    
    [Test]
    public async Task CreateAdmin_AdminWithValidData_CreatesAdminAndAccount()
    {
        // Arrange
        var dto = AdminTestData.CreateDto();
        
        // Act
        await GetServiceManager().AdminService.CreateAsync(dto);

        // Assert
        // Has unique Admin record
        var admin = GetDbContext().Admins.Single(a => 
            a.FirstName == dto.FirstName &&
            a.LastName == dto.LastName &&
            a.Gender == dto.Gender &&
            a.Address == dto.Address &&
            a.Phone == dto.Phone &&
            a.Email == dto.Email &&
            a.DateOfBirth == dto.DateOfBirth);

        // Has unique Account record
        var account = GetDbContext().Accounts.Single(a => a.UserId == admin!.Id);

        // Has unique Identity record
        var identityUser = GetDbContext().Users.Single(a => a.Id == account!.IdentityUserId);

        // Has expected username
        Assert.That(identityUser!.UserName, Is.EqualTo(AdminTestData.ExpectedUsername));
        
        // Has expected AuthRole
        var adminRoleId = GetDbContext().Roles.Single(r => r.Name == AuthRoles.Admin).Id;
        Assert.That(GetDbContext().UserRoles.Any(r => r.UserId == identityUser.Id && r.RoleId == adminRoleId));
    }

    [Test]
    public void CreateAsync_AdminWithInvalidData_Throws()
    {
        // Arrange
        var dto = AdminTestData.CreateDto();
        dto.FirstName = string.Empty;
        dto.Phone = string.Empty;
        dto.Email = string.Empty;
        
        // Act & Assert
        Assert.ThrowsAsync<AdminBadRequest>(async () => await GetServiceManager().AdminService.CreateAsync(dto));
    }
    
    [Test]
    public async Task CreateAsync_AdminWithDuplicateData_Throws()
    {
        // Arrange
        await AdminTestData.SeedAdmin(GetDbContext(), GetIdentityUserManager());
        var dto = AdminTestData.CreateDto();
        
        // Act & Assert
        Assert.ThrowsAsync<AdminBadRequest>(async () => await GetServiceManager().AdminService.CreateAsync(dto));
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingAdmin_ReturnsAdmin()
    {
        // Arrange
        var seededAdminDto = await AdminTestData.SeedAdmin(GetDbContext(), GetIdentityUserManager());
        
        // Act
        var dto = await GetServiceManager().AdminService.GetByIdAsync(seededAdminDto.Id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dto.FirstName, Is.EqualTo(seededAdminDto.FirstName));
            Assert.That(dto.LastName, Is.EqualTo(seededAdminDto.LastName));
            Assert.That(dto.Gender, Is.EqualTo(seededAdminDto.Gender));
            Assert.That(dto.Address, Is.EqualTo(seededAdminDto.Address));
            Assert.That(dto.Phone, Is.EqualTo(seededAdminDto.Phone));
            Assert.That(dto.Email, Is.EqualTo(seededAdminDto.Email));
            Assert.That(dto.DateOfBirth, Is.EqualTo(seededAdminDto.DateOfBirth));
        });
    }
    
    [Test]
    public void GetByIdAsync_NonExistingAdmin_Throws()
    {
        // Act & Assert
        Assert.ThrowsAsync<AdminNotFoundException>(async () => 
            await GetServiceManager().AdminService.GetByIdAsync(Guid.NewGuid()));
    }
    
    [Test]
    public async Task UpdateAsync_ExistingAdminWithValidData_UpdatesAdmin()
    {
        // Arrange
        var seededAdminDto = await AdminTestData.SeedAdmin(GetDbContext(), GetIdentityUserManager());
        seededAdminDto.FirstName = "UpdatedFirstName";
        seededAdminDto.Email = "updatedEmail@example.com";
        
        // Act
        await GetServiceManager().AdminService.UpdateAsync(seededAdminDto);
        
        // Assert
        var result = GetDbContext().Admins.Single(a => a.Id == seededAdminDto.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result!.FirstName, Is.EqualTo(seededAdminDto.FirstName));
            Assert.That(result.Email, Is.EqualTo(seededAdminDto.Email));
        });
    }
    
    [Test]
    public async Task UpdateAsync_ExistingAdminWithInvalidData_Throws()
    {
        // Arrange
        var seededAdminDto = await AdminTestData.SeedAdmin(GetDbContext(), GetIdentityUserManager());
        seededAdminDto.FirstName = "";
        seededAdminDto.Email = "";
        
        // Act & Assert
        Assert.ThrowsAsync<AdminBadRequest>(() => GetServiceManager().AdminService.UpdateAsync(seededAdminDto));
    }
    
    [Test]
    public void UpdateAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var dto = AdminTestData.CreateDto().Adapt<AdminDto>();
        dto.Id = Guid.NewGuid();
        
        // Act & Assert
        Assert.ThrowsAsync<AdminNotFoundException>(() => GetServiceManager().AdminService.UpdateAsync(dto));
    }
    
    [Test]
    public async Task DeleteAsync_ExistingAdmin_DeletesAdminAndAccount()
    {
        // Arrange
        var seededAdminDto = await AdminTestData.SeedAdmin(GetDbContext(), GetIdentityUserManager());
        var context = GetDbContext();
        
        // Act
        await GetServiceManager().AdminService.DeleteAsync(seededAdminDto.Id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(context.Admins, Is.Empty);
            Assert.That(context.Accounts, Is.Empty);
            Assert.That(context.Users, Is.Empty);
            Assert.That(context.UserRoles, Is.Empty);
        });
    }
    
    [Test]
    public void DeleteAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act & Assert
        Assert.ThrowsAsync<AdminNotFoundException>(() => GetServiceManager().AdminService.DeleteAsync(id));
    }
}