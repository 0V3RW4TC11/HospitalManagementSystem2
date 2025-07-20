using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Services;
using HospitalManagementSystem2.Tests.Helpers;
using HospitalManagementSystem2.Tests.TestData;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HospitalManagementSystem2.Tests.Services;

public class AdminServiceIntegrationTests : IDisposable, IAsyncDisposable
{
    // Services
    private readonly SqliteInMemDbHelper _dbHelper;

    public AdminServiceIntegrationTests()
    {
        _dbHelper = new SqliteInMemDbHelper(services =>
        {
            services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<AccountService>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IStaffEmailGenerator, StaffEmailGenerator>();
            services.AddScoped<AdminService>();
        });
        
        SeedAdminRole();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbHelper.DisposeAsync();
    }

    public void Dispose()
    {
        _dbHelper.Dispose();
    }

    private void SeedAdminRole()
    {
        using var scope = _dbHelper.ServiceProvider.CreateScope();
        var roleMan = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        roleMan.CreateAsync(new IdentityRole(Constants.AuthRoles.Admin)).GetAwaiter().GetResult();
    }
    
    private ApplicationDbContext GetContext() => _dbHelper.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    private AdminService GetSut() => _dbHelper.ServiceProvider.GetRequiredService<AdminService>();
    
    private string GetAdminRoleId() => GetContext().Roles.Single(r => r.Name == Constants.AuthRoles.Admin).Id;

    private Admin CreateAdminAccount()
    {
        var context = GetContext();
        var admin = AdminTestHelper.CreateAndSeedAdmin(context);
        AccountTestHelper.SeedAccount(context, 
                                      _dbHelper.ServiceProvider, 
                                      admin.Id, 
                                      StaffTestHelper.ExpectedOrgEmail(admin.FirstName, admin.LastName!), 
                                      PersonTestData.TestPassword,
                                      Constants.AuthRoles.Admin);
        return admin;
    }

    [Fact]
    public async Task CreateAsync_NewAdmin_AddsToDatabase()
    {
        // Arrange
        var context = _dbHelper.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var admin = AdminTestHelper.CreateAdmin();

        // Act
        await GetSut().CreateAsync(admin, PersonTestData.TestPassword);

        // Assert
        AdminTestHelper.AssertHasData(context, admin);
        AccountTestHelper.AssertHasAccount(context, admin.Id, GetAdminRoleId());
    }

    [Fact]
    public async Task CreateAsync_NewAdminExistingDetails_Throws()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAdmin();
        CreateAdminAccount();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(admin, PersonTestData.TestPassword));
        Assert.Contains(ErrorMessageData.DuplicateRecord, result.Message);
    }

    [Fact]
    public async Task CreateAsync_NewAdminInvalidData_Throws()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAdmin();
        admin.FirstName = null!;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(admin, PersonTestData.TestPassword));
        Assert.Contains(ErrorMessageData.EntityChangesError, result.Message);;
    }

    [Fact]
    public async Task CreateAsync_NullAdmin_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(null!, PersonTestData.TestPassword));
        Assert.Contains(ErrorMessageData.LinqQueryExceptionThrown, result.Message);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAdmin_UpdatesAdmin()
    {
        // Arrange
        var context = GetContext();
        var admin = CreateAdminAccount();
        admin.FirstName = "NewFirstName";
        
        // Act
        await GetSut().UpdateAsync(admin);
        
        // Assert
        AdminTestHelper.AssertHasData(context, admin);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAdminInvalidData_Throws()
    {
        // Arrange
        var context = GetContext();
        var admin = CreateAdminAccount();
        admin.FirstName = null!;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(admin));
        Assert.Contains(ErrorMessageData.EntityChangesError, result.Message);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAdmin();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(admin));
        Assert.Contains(ErrorMessageData.SequenceNoElements, result.Message);
    }

    [Fact]
    public async Task UpdateAsync_NullAdmin_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(null!));
        Assert.Contains(ErrorMessageData.LinqQueryExceptionThrown, result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ExistingAdmin_RemovesAdmin()
    {
        // Arrange
        var context = GetContext();
        var admin = CreateAdminAccount();
        var account = AccountTestHelper.GetAccount(context, admin.Id);
        
        // Act
        await GetSut().DeleteAsync(admin);
        
        // Assert
        AdminTestHelper.AssertHasNoData(context, admin);
        AccountTestHelper.AssertHasNoAccount(context, account);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAdmin();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().DeleteAsync(admin));
        Assert.Contains(ErrorMessageData.SequenceNoElements, result.Message);
    }

    [Fact]
    public async Task DeleteAsync_NullAdmin_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().DeleteAsync(null!));
        Assert.Contains(ErrorMessageData.LinqQueryExceptionThrown, result.Message);   
    }
}