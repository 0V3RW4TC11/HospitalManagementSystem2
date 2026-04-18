using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;

namespace Seeders.Services
{
    internal class DatabaseService
    {
        public IServiceProvider Services { get; }

        public DatabaseService()
        {
            // Create configuration
            var config = CreateConfiguration();
            // Create service provider
            var services = CreateServiceProvider(config);
            
            Services = services;
        }

        public async Task InitializeDatabaseAsync()
        {
            var database = Services.GetRequiredService<HmsDbContext>().Database;
            await database.MigrateAsync();
        }

        private static IConfiguration CreateConfiguration()
        {
            var webProjectDir = Path.Combine(AppContext.BaseDirectory, "../../../..", "Web");
            return new ConfigurationBuilder()
                .SetBasePath(webProjectDir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static ServiceProvider CreateServiceProvider(IConfiguration config)
        {
            // Create service collection
            var services = new ServiceCollection();

            // Get connection string
            var connectionName = "DefaultConnection";
            var connectionString = config.GetConnectionString(connectionName) ??
                throw new InvalidOperationException($"Connection string '{connectionName}' not found.");

            // Add null logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(NullLoggerProvider.Instance);
            });

            // Add DbContext
            services.AddDbContext<HmsDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add DbContextFactory
            services.AddDbContextFactory<HmsDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add Asp Identity Middleware
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<HmsDbContext>()
                .AddDefaultTokenProviders();

            // Build and return service provider
            return services.BuildServiceProvider();
        }
    }
}
