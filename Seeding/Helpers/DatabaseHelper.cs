using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;
using Persistence.Repositories;
using Services;
using Services.Abstractions;

namespace Seeding.Helpers
{
    internal static class DatabaseHelper
    {
        public static async Task<ServiceProvider> SetupAndConfigureDatabase()
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

            var provider = services.BuildServiceProvider();

            // Ensure database is created and migrations applied
            var context = provider.GetRequiredService<RepositoryDbContext>();
            await context.Database.MigrateAsync();

            return provider;
        }

        private static void Configure(ServiceCollection services, IConfiguration config)
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
    }
}
