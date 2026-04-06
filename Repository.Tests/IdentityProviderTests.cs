using Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Persistence.Tests2
{
    [TestFixture]
    internal class IdentityProviderTests
    {
        private ServiceProvider _serviceProvider;

        private RoleManager<IdentityRole> RoleManager => _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        private UserManager<IdentityUser> UserManager => _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        private IIdentityProvider IdentityProvider => _serviceProvider.GetRequiredService<IIdentityProvider>();

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

            // Add Identity provider
            services.AddScoped<IIdentityProvider, IdentityProvider>();

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
        public async Task CreateIdentityAsync_Succeeds()
        {
            // Arrange
            var userManager = UserManager;
            var hmsUserId = Guid.NewGuid();
            var userName = "testuser";
            var password = "TestPassword123!";
            var role = "Admin";
            await RoleManager.CreateAsync(new IdentityRole(role));

            // Act
            await IdentityProvider.CreateIdentityAsync(hmsUserId, userName, password, role, CancellationToken.None);

            // Assert
            var user = await userManager.FindByNameAsync(userName);
            Assert.Multiple(async () =>
            {
                Assert.That(user, Is.Not.Null);
                Assert.That(await userManager.IsInRoleAsync(user!, role), Is.True);
            });

            var claims = await userManager.GetClaimsAsync(user);
            var hmsClaim = claims.FirstOrDefault(c => c.Type == "hms_user_id");
            Assert.Multiple(() =>
            {
                Assert.That(hmsClaim, Is.Not.Null);
                Assert.That(Guid.Parse(hmsClaim!.Value), Is.EqualTo(hmsUserId));
            });
        }

        [Test]
        public async Task DeleteIdentityAsync_Succeeds()
        {
            // Arrange
            var userManager = UserManager;
            var hmsUserId = Guid.NewGuid();
            var userName = "testuser";
            var password = "TestPassword123!";
            var role = "Admin";
            await RoleManager.CreateAsync(new IdentityRole(role));
            var identityProvider = IdentityProvider;
            await identityProvider.CreateIdentityAsync(hmsUserId, userName, password, role, CancellationToken.None);

            // Act
            await identityProvider.DeleteIdentityAsync(hmsUserId, CancellationToken.None);

            // Assert
            var user = await userManager.FindByNameAsync(userName);
            Assert.That(user, Is.Null);
        }
    }
}
