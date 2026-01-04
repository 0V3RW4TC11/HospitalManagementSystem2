using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;

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

            services.AddDbContextFactory<RepositoryDbContext>(options =>
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
        }

        public static async Task<bool> HasDataAsync(RepositoryDbContext context)
        {
            if (await context.Accounts.AnyAsync())
                return true;
            if (await context.Admins.AnyAsync())
                return true;
            if (await context.Patients.AnyAsync())
                return true;
            if (await context.Doctors.AnyAsync())
                return true;
            if (await context.DoctorSpecializations.AnyAsync())
                return true;
            if (await context.Specializations.AnyAsync())
                return true;
            if (await context.Attendances.AnyAsync())
                return true;
            if (await context.Users.AnyAsync())
                return true;
            
            return false;
        }

        public static async Task ResetDatabase(RepositoryDbContext context)
        {
            await Task.WhenAll(
                context.Accounts.ExecuteDeleteAsync(),
                context.Admins.ExecuteDeleteAsync(),
                context.Patients.ExecuteDeleteAsync(),
                context.Doctors.ExecuteDeleteAsync(),
                context.DoctorSpecializations.ExecuteDeleteAsync(),
                context.Specializations.ExecuteDeleteAsync(),
                context.Attendances.ExecuteDeleteAsync(),
                context.Users.ExecuteDeleteAsync()
            );
        }
    }
}
