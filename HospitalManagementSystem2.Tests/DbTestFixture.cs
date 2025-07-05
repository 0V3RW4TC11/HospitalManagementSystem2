using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HospitalManagementSystem2.Tests
{
    public class DbTestFixture : IDisposable
    {
        public readonly ServiceProvider ServiceProvider;
        private readonly SqliteConnection _connection;

        public DbTestFixture() 
        {
            // Keep the SQLite connection open for the in-memory database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Configure services
            var services = new ServiceCollection();

            // Add logging (NullLogger for simplicity)
            services.AddLogging(builder => builder.AddProvider(new NullLoggerProvider()));

            // Add EF Core with in-memory SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_connection));

            // Add Identity services
            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add Scoped services
            services.AddScoped<AccountHelper>();

            // Build the service provider
            ServiceProvider = services.BuildServiceProvider();

            // Ensure the database is created
            var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        public async Task ResetDatabaseAsync()
        {
            var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }

        public void Dispose()
        {
            _connection.Close();
            ServiceProvider.Dispose();
        }
    }

    // NullLoggerProvider implementation
    public class NullLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => NullLogger.Instance;

        public void Dispose() { }

        private class NullLogger : ILogger
        {
            public static readonly NullLogger Instance = new NullLogger();

            public IDisposable BeginScope<TState>(TState state) => NullDisposable.Instance;

            public bool IsEnabled(LogLevel logLevel) => false;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
        }

        private class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new NullDisposable();
            public void Dispose() { }
        }
    }
}
