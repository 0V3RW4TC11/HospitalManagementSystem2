using HospitalManagementSystem2.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HospitalManagementSystem2.Tests.Helpers;

public class SqliteInMemDbHelper : IDisposable, IAsyncDisposable
{
    public readonly IServiceProvider ServiceProvider;
    private readonly SqliteConnection _connection;

    public SqliteInMemDbHelper(Action<ServiceCollection> serviceConfig)
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
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(_connection));

        // Add Identity services for UserManager<IdentityUser>
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        // Add services
        serviceConfig.Invoke(services);
        
        // Build service provider
        ServiceProvider = services.BuildServiceProvider();

        // Ensure the database schema is created (including Identity tables)
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        (ServiceProvider as IDisposable).Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await (ServiceProvider as IAsyncDisposable).DisposeAsync();
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }
}