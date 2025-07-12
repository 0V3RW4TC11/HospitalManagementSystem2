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
    private const string TestTitle = "ExampleTitle";
    private const string TestFirstName = "ExampleFirstName";
    private const string TestLastName = "ExampleLastName";
    private const string TestGender = "ExampleGender";
    private const string TestAddress = "ExampleAddress";
    private const string TestPhone = "ExamplePhone";
    private const string TestEmail = "ExampleEmail";
    private const string TestPassword = "TestPass123!";

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
    public async Task CreateAsync_NewAdmin_ShouldAddToDatabase()
    {
        // Arrange
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
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userMan = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var sut = _serviceProvider.GetRequiredService<AdminService>();

        // Act
        await sut.CreateAsync(admin, TestPassword);

        // Assert
        Assert.NotEqual(admin.Id, Guid.Empty);
        Assert.Contains(context.Admins, a => a.Id == admin.Id);
        var account = context.Accounts.FirstOrDefault(a => a.UserId == admin.Id);
        Assert.NotNull(account);
        var identityUser = context.Users.FirstOrDefault(u => u.Id == account.IdentityUserId);
        Assert.NotNull(identityUser);
        Assert.True(await userMan.IsInRoleAsync(identityUser, Constants.AuthRoles.Admin));
    }
}