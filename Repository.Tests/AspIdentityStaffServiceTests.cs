using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Persistence.Tests2
{
    [TestFixture]
    internal class AspIdentityStaffServiceTests
    {
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            // Set up a service collection
            var services = new ServiceCollection();

            // Add null logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(NullLoggerProvider.Instance);
            });

            // Configure DbContext with EF Core In-Memory
            services.AddDbContext<RepositoryDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            // Add Identity services
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<RepositoryDbContext>()
                .AddDefaultTokenProviders();

            // Build service provider
            _serviceProvider = services.BuildServiceProvider();

            // Ensure the database schema is created
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
            context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
        }

        [Test]
        // a.b doesnt exist in any form and should be returned
        public async Task CreateStaffUsernameAsync_UniqueName_Succeeds()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            var staffService = new AspIdentityStaffService(context);

            // Act
            var result = await staffService.CreateStaffUsernameAsync("a", "b");

            // Assert
            Assert.That(result, Is.EqualTo("a.b@sjog.org.au"));
        }

        [Test]
        // only a.b exists
        // a.b2 should be returned
        public async Task CreateStaffUsernameAsync_SimilarName_Succeeds()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            context.Users.Add(new IdentityUser { UserName = "a.b@sjog.org.au" });
            context.SaveChanges();
            var staffService = new AspIdentityStaffService(context);

            // Act
            var result = await staffService.CreateStaffUsernameAsync("a", "b");

            // Assert
            Assert.That(result, Is.EqualTo("a.b2@sjog.org.au"));
        }

        [Test]
        // a.b, a.b2, a.b3 were created but a.b2 got deleted.
        // a.b4 should be returned
        public async Task CreateStaffUsernameAsync_SimilarNameButSkippedIteration_Succeeds()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            context.Users.AddRange(
                new IdentityUser { UserName = "a.b@sjog.org.au" },
                new IdentityUser { UserName = "a.b3@sjog.org.au" }
            );
            context.SaveChanges();
            var staffService = new AspIdentityStaffService(context);

            // Act
            var result = await staffService.CreateStaffUsernameAsync("a", "b");

            // Assert
            Assert.That(result, Is.EqualTo("a.b4@sjog.org.au"));
        }
    }
}
