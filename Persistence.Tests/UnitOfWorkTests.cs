using Abstractions;
using Domain.Entities;
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
    internal class UnitOfWorkTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        private RepositoryDbContext Context => _serviceProvider.GetRequiredService<RepositoryDbContext>();
        private IUnitOfWork UnitOfWork => _serviceProvider.GetRequiredService<IUnitOfWork>();

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

            // Configure ApplicationDbContext with SQLite in-memory
            services.AddDbContext<RepositoryDbContext>(options =>
                options.UseSqlite(_connection));

            // Add Identity services
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<RepositoryDbContext>()
                .AddDefaultTokenProviders();

            // Add services
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Build service provider
            _serviceProvider = services.BuildServiceProvider();

            // Ensure the database schema is created (including Identity tables)
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
            context.Database.EnsureCreated();
        }

        [TearDown]
        public void Teardown()
        {
            _serviceProvider.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        public async Task SaveChangesAsync_NoChangesMade_Succeeds()
        {
            // Act
            await UnitOfWork.SaveChangesAsync();

            // Assert
            // No exception thrown, succeeds
        }

        [Test]
        public async Task SaveChangesAsync_ChangesMade_Succeeds()
        {
            // Arrange
            var admin = new Admin { FirstName = "Test", Email = "test@example.com", Phone = "123" };

            // Act
            await UnitOfWork.Admins.AddAsync(admin);
            await UnitOfWork.SaveChangesAsync();

            // Assert
            var savedAdmin = await Context.Admins.FindAsync(admin.Id);
            Assert.That(savedAdmin, Is.Not.Null);
            Assert.That(savedAdmin.FirstName, Is.EqualTo("Test"));
        }

        [Test]
        public async Task RunInTransactionAsync_OperationDoesNotThrow_ChangesPersistedToDatabase()
        {
            // Arrange
            var admin = new Admin { FirstName = "Test", Email = "test@example.com", Phone = "123" };

            // Act
            await UnitOfWork.RunInTransactionAsync(async ct =>
            {
                await UnitOfWork.Admins.AddAsync(admin, ct);
                await UnitOfWork.SaveChangesAsync(ct);
            });

            // Assert
            var savedAdmin = await Context.Admins.FindAsync(admin.Id);
            Assert.That(savedAdmin, Is.Not.Null);
            Assert.That(savedAdmin.FirstName, Is.EqualTo("Test"));
        }

        [Test]
        public async Task RunInTransactionAsync_OperationThrows_ChangesNotPersistedToDatabase()
        {
            // Arrange
            var admin = new Admin { FirstName = "Test", Email = "test@example.com", Phone = "123" };
            var admin2 = new Admin { FirstName = "Test2", Email = "test2@example.com", Phone = "123" };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
            {
                await UnitOfWork.RunInTransactionAsync(async ct =>
                {
                    await UnitOfWork.Admins.AddAsync(admin, ct);
                    await UnitOfWork.SaveChangesAsync(ct);
                    await UnitOfWork.Admins.AddAsync(admin2, ct);
                    throw new Exception("Test exception");
                });
            });

            // Assert
            var savedAdmin = await Context.Admins.FindAsync(admin.Id);
            var savedAdmin2 = await Context.Admins.FindAsync(admin2.Id);
            Assert.Multiple(() =>
            {
                Assert.That(savedAdmin, Is.Null);
                Assert.That(savedAdmin2, Is.Null);
            });
        }
    }
}
