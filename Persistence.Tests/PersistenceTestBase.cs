using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence.Repositories;
using Services;
using Services.Abstractions;

namespace Persistence.Tests;

internal abstract class PersistenceTestBase
{
    private ServiceProvider _serviceProvider;
    private SqliteConnection _connection;

    [SetUp]
    public virtual void SetUp()
    {
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
        services.AddDbContext<RepositoryDbContext>(options =>
            options.UseSqlite(_connection));

        // Add Identity services for UserManager<IdentityUser>
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<RepositoryDbContext>()
            .AddDefaultTokenProviders();
        
        // Add services
        services.AddScoped<IRepositoryManager, RepositoryManager>();
        services.AddScoped<IServiceManager, ServiceManager>();
        
        // Build service provider
        _serviceProvider = services.BuildServiceProvider();

        // Ensure the database schema is created (including Identity tables)
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
        context.Database.EnsureCreated();
    }
    
    [TearDown]
    public virtual void TearDown()
    {
        _serviceProvider.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    protected IServiceProvider GetServiceProvider() => _serviceProvider;
    
    protected RepositoryDbContext GetDbContext() => _serviceProvider.GetRequiredService<RepositoryDbContext>();
    
    protected UserManager<IdentityUser> GetIdentityUserManager() => 
        _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    
    protected IServiceManager GetServiceManager() => _serviceProvider.GetRequiredService<IServiceManager>();
}