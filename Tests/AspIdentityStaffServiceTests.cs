using Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;

namespace Tests
{
    [TestFixture]
    internal class AspIdentityStaffServiceTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        private HmsDbContext Context => _serviceProvider.GetRequiredService<HmsDbContext>();
        private StaffService StaffService => _serviceProvider.GetRequiredService<StaffService>();

        [SetUp]
        public void Setup()
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

            // Configure DbContext
            services.AddDbContext<HmsDbContext>(options =>
                options.UseSqlite(_connection));

            // Add Identity services
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<HmsDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<StaffService, AspIdentityStaffService>();

            // Build service provider
            _serviceProvider = services.BuildServiceProvider();

            // Ensure the database schema is created
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HmsDbContext>();
            context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        // a.b doesnt exist in any form and should be returned
        public async Task CreateStaffUsernameAsync_UniqueName_Succeeds()
        {
            // Act
            var result = await StaffService.CreateStaffUsernameAsync("a", "b");

            // Assert
            Assert.That(result, Is.EqualTo("a.b@sjog.org.au"));
        }

        [Test]
        // only a.b exists
        // a.b2 should be returned
        public async Task CreateStaffUsernameAsync_SimilarName_Succeeds()
        {
            // Arrange
            var context = Context;
            context.Users.Add(new IdentityUser { UserName = "a.b@sjog.org.au" });
            context.SaveChanges();

            // Act
            var result = await StaffService.CreateStaffUsernameAsync("a", "b");

            // Assert
            Assert.That(result, Is.EqualTo("a.b2@sjog.org.au"));
        }

        [Test]
        // a.b, a.b2, a.b3 were created but a.b2 got deleted.
        // a.b4 should be returned
        public async Task CreateStaffUsernameAsync_SimilarNameButSkippedIteration_Succeeds()
        {
            // Arrange
            var context = Context;
            context.Users.AddRange(
                new IdentityUser { UserName = "a.b@sjog.org.au" },
                new IdentityUser { UserName = "a.b3@sjog.org.au" }
            );
            context.SaveChanges();

            // Act
            var result = await StaffService.CreateStaffUsernameAsync("a", "b");

            // Assert
            Assert.That(result, Is.EqualTo("a.b4@sjog.org.au"));
        }
    }
}
