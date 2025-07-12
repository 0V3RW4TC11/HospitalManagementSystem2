using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HospitalManagementSystem2.Tests.Services;

public class AdminServiceIntegrationTests : IDisposable, IAsyncDisposable
{
    // Test Admin Details
    private const string TestTitle = "TestTitle";
    private const string TestFirstName = "TestFirstName";
    private const string TestLastName = "TestLastName";
    private const string TestGender = "TestGender";
    private const string TestAddress = "TestAddress";
    private const string TestPhone = "TestPhone";
    private const string TestEmail = "TestEmail";
    private const string TestPassword = "TestPass123!";
    private static readonly string ExpectedOrgEmail 
        = $"{TestFirstName.ToLower()}.{TestLastName.ToLower()}@{Constants.StaffEmailDomain}";

    private static readonly DateOnly TestDateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch);

    // Services
    private readonly SqliteConnection _connection;
    private readonly IServiceProvider _serviceProvider;

    public AdminServiceIntegrationTests()
    {
        // Create and open a shared SQLite in-memory connection
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // Set up a service collection
        var services = new ServiceCollection();

        // Add null logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(NullLoggerProvider.Instance);
        });

        // Configure ApplicationDbContext with SQLite in-memory
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(_connection));

        // Add Identity services for UserManager<IdentityUser>
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Add scoped services
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<AccountService>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IStaffEmailGenerator, StaffEmailGenerator>();
        services.AddScoped<AdminService>();

        // Build service provider
        _serviceProvider = services.BuildServiceProvider();

        // Ensure the database schema is created (including Identity tables)
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();

        // Inject ADMIN role
        var roleMan = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        roleMan.CreateAsync(new IdentityRole(Constants.AuthRoles.Admin)).GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        await (_serviceProvider as IAsyncDisposable).DisposeAsync();
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }

    public void Dispose()
    {
        (_serviceProvider as IDisposable).Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task CreateAsync_NewAdmin_AddsToDatabase()
    {
        // Arrange
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userMan = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };

        // Act
        await sut.CreateAsync(admin, TestPassword);

        // Assert
        Assert.NotEqual(Guid.Empty, admin.Id);
        Assert.Contains(context.Admins, a => a.Id == admin.Id);
        var account = context.Accounts.FirstOrDefault(a => a.UserId == admin.Id);
        Assert.NotNull(account);
        var identityUser = context.Users.FirstOrDefault(u => u.Id == account.IdentityUserId);
        Assert.NotNull(identityUser);
        Assert.Equal(ExpectedOrgEmail, identityUser.UserName);
        Assert.True(await userMan.IsInRoleAsync(identityUser, Constants.AuthRoles.Admin));
    }

    [Fact]
    public async Task CreateAsync_AdminWithExistingDetails_ThrowsWithMessage()
    {
        // Arrange
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var otherAdmin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var expectedMessage = "A duplicate record exists";

        context.Admins.Add(admin);
        context.SaveChanges();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateAsync(otherAdmin, TestPassword));
        Assert.Equal(expectedMessage, result.Message);
        Assert.Equal(Guid.Empty, otherAdmin.Id);
        Assert.DoesNotContain(context.Admins, a => a == otherAdmin);
        Assert.Empty(context.Accounts);
        Assert.Empty(context.Users);
    }

    [Fact]
    public async Task CreateAsync_AdminWithRequiredFieldMissing_Throws()
    {
        // Arrange
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = null,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateAsync(admin, TestPassword));
        Assert.Empty(context.Admins);
        Assert.Empty(context.Accounts);
        Assert.Empty(context.Users);
    }
    
    [Fact]
    public async Task CreateAsync_AdminWithUnrequiredFieldMissing_AddsToDatabase()
    {
        // Arrange
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userMan = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = null,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        
        // Act & Assert
        await sut.CreateAsync(admin, TestPassword);
        
        // Assert
        Assert.NotEqual(Guid.Empty, admin.Id);
        Assert.Contains(context.Admins, a => a.Id == admin.Id);
        var account = context.Accounts.FirstOrDefault(a => a.UserId == admin.Id);
        Assert.NotNull(account);
        var identityUser = context.Users.FirstOrDefault(u => u.Id == account.IdentityUserId);
        Assert.NotNull(identityUser);
        Assert.True(await userMan.IsInRoleAsync(identityUser, Constants.AuthRoles.Admin));
    }

    [Fact]
    public async Task CreateAsync_NullAdmin_Throws()
    {
        // Arrange
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateAsync(null, TestPassword));
        Assert.Empty(context.Admins);
        Assert.Empty(context.Accounts);
        Assert.Empty(context.Users);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAdmin_UpdatesAdmin()
    {
        // Arrange
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        context.Admins.Add(admin);
        context.SaveChanges();
        admin.FirstName = "AnotherName";
        
        // Act
        await sut.UpdateAsync(admin);
        
        // Assert
        var result = context.Admins.FirstOrDefault(a => a.Id == admin.Id);
        Assert.NotNull(result);
        Assert.Equal(admin.FirstName, result.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(admin));
    }

    [Fact]
    public async Task UpdateAsync_AdminWithMissingRequiredDetails_Throws()
    {
        // Arrange
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        context.Admins.Add(admin);
        context.SaveChanges();
        admin.FirstName = null;
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(admin));
    }

    [Fact]
    public async Task UpdateAsync_NullAdmin_Throws()
    {
        // Arrange
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(null));
    }

    [Fact]
    public async Task DeleteAsync_CreateAdminThenDelete_RemovesAdminAndAccount()
    {
        // Arrange
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        
        // Act
        await sut.CreateAsync(admin, TestPassword);
        await sut.DeleteAsync(admin);
        
        // Assert
        Assert.Empty(context.Admins);
        Assert.Empty(context.Accounts);
        Assert.Empty(context.Users);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.DeleteAsync(admin));
    }

    [Fact]
    public async Task DeleteAsync_NullAdmin_Throws()
    {
        // Arrange
        var sut = _serviceProvider.GetRequiredService<AdminService>();
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.DeleteAsync(null));
    }
}