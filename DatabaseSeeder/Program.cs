using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;
using Persistence.Repositories;
using Seeding;
using Services;
using Services.Abstractions;

try
{
    // Build configuration
    var solutionDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../.."));
    var webProjectDir = Path.Combine(solutionDir, "Web");
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(webProjectDir)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    // Setup DI
    var services = new ServiceCollection();
    Configure(services, config);

    using var serviceProvider = services.BuildServiceProvider();
    
    // Resolve services
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var serviceManager = serviceProvider.GetRequiredService<IServiceManager>();
    var context = serviceProvider.GetRequiredService<RepositoryDbContext>();

    // Ensure database is created and migrations applied
    await context.Database.MigrateAsync();

    // Seed data
    Console.WriteLine("Seeding authentication roles ...");
    await SeedData.SeedAuthRoles(roleManager);

    Console.WriteLine("Seeding admin user ...");
    await SeedData.SeedAdmin(serviceManager.AdminService);

    Console.WriteLine("Database seeding completed");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    if (ex.InnerException != null )
    {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
}

void Configure(ServiceCollection services, IConfiguration config)
{
    // Add DbContext
    var connectionName = "DefaultConnection";
    var connectionString = config.GetConnectionString(connectionName) ??
        throw new InvalidOperationException($"Connection string '{connectionName}' not found.");

    services.AddDbContext<RepositoryDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Add null logging
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddProvider(NullLoggerProvider.Instance);
    });

    // Add Identity
    services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<RepositoryDbContext>()
        .AddDefaultTokenProviders();

    // Add application services
    services.AddScoped<IRepositoryManager, RepositoryManager>();
    services.AddScoped<IServiceManager, ServiceManager>();
}