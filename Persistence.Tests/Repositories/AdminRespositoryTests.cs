using Ardalis.Specification;
using Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;
using Specifications.Admin;

namespace Tests.Repositories
{
    internal class AdminSingleByEmailSpec : SingleResultSpecification<Admin>
    {
        public AdminSingleByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();
            Query.Where(a => a.Email.ToLower() == normalizedEmail);
        }
    }

    [TestFixture]
    internal class AdminRespositoryTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        private HmsDbContext Context => _serviceProvider.GetRequiredService<HmsDbContext>();

        private static Admin CreateAdmin() => new()
        {
            FirstName = "ExampleFirstName",
            LastName = "ExampleLastName",                               // nullable
            Gender = "Male",                                            // nullable
            Address = "ExampleAddress",                                 // nullable
            Phone = "123-456-7890",
            Email = "admin@example.com",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),    // nullable
        };

        private static Admin CreateAdmin(string email) => new()
        {
            FirstName = "ExampleFirstName",
            LastName = "ExampleLastName",
            Gender = "Male",
            Address = "ExampleAddress",
            Phone = "123-456-7890",
            Email = email,
            DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),
        };

        [SetUp]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(NullLoggerProvider.Instance);
            });

            services.AddDbContext<HmsDbContext>(options =>
                options.UseSqlite(_connection));

            _serviceProvider = services.BuildServiceProvider();

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
        public async Task AddAsync_WithValidData_Succeeds()
        {
            // Arrange
            var admin = CreateAdmin();
            var repo = new Repository<Admin>(Context);

            // Act
            await repo.AddAsync(admin);
            Context.SaveChanges();

            // Assert
            var result = Context.Admins.Single(a => a.Id == admin.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.FirstName, Is.EqualTo(admin.FirstName));
                Assert.That(result.LastName, Is.EqualTo(admin.LastName));
                Assert.That(result.Gender, Is.EqualTo(admin.Gender));
                Assert.That(result.Address, Is.EqualTo(admin.Address));
                Assert.That(result.Phone, Is.EqualTo(admin.Phone));
                Assert.That(result.Email, Is.EqualTo(admin.Email));
                Assert.That(result.DateOfBirth, Is.EqualTo(admin.DateOfBirth));
            });
        }

        [Test]
        public async Task AddRangeAsync_WithValidData_Succeeds()
        {
            // Arrange
            var admins = new List<Admin> { CreateAdmin("admin1@example.com"), CreateAdmin("admin2@example.com") };
            var repo = new Repository<Admin>(Context);

            // Act
            await repo.AddRangeAsync(admins);
            Context.SaveChanges();

            // Assert
            var results = Context.Admins.Where(a => admins.Select(x => x.Id).Contains(a.Id)).ToList();
            Assert.That(results, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task AnyAsync_WithoutSpec_ReturnsTrueWhenEntitiesExist()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);

            // Act
            var result = await repo.AnyAsync();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task AnyAsync_WithoutSpec_ReturnsFalseWhenNoEntities()
        {
            // Arrange
            var repo = new Repository<Admin>(Context);

            // Act
            var result = await repo.AnyAsync();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AnyAsync_WithSpec_ReturnsTrueWhenMatches()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminByEmailSpec(admin.Email);

            // Act
            var result = await repo.AnyAsync(spec);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task AnyAsync_WithSpec_ReturnsFalseWhenNoMatches()
        {
            // Arrange
            var repo = new Repository<Admin>(Context);
            var spec = new AdminByEmailSpec("nonexistent@example.com");

            // Act
            var result = await repo.AnyAsync(spec);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CountAsync_WithoutSpec_ReturnsCorrectCount()
        {
            // Arrange
            var admins = new List<Admin> { CreateAdmin("admin1@example.com"), CreateAdmin("admin2@example.com") };
            Context.Admins.AddRange(admins);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);

            // Act
            var result = await repo.CountAsync();

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task CountAsync_WithSpec_ReturnsCorrectCount()
        {
            // Arrange
            var admin1 = CreateAdmin("admin1@example.com");
            var admin2 = CreateAdmin("admin2@example.com");
            Context.Admins.AddRange([admin1, admin2]);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminByEmailSpec(admin1.Email);

            // Act
            var result = await repo.CountAsync(spec);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task FirstOrDefaultAsync_WithSpec_ReturnsEntityWhenExists()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminByEmailSpec(admin.Email);

            // Act
            var result = await repo.FirstOrDefaultAsync(spec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(admin.Id));
        }

        [Test]
        public async Task FirstOrDefaultAsync_WithSpec_ReturnsNullWhenNotExists()
        {
            // Arrange
            var repo = new Repository<Admin>(Context);
            var spec = new AdminByEmailSpec("nonexistent@example.com");

            // Act
            var result = await repo.FirstOrDefaultAsync(spec);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task FirstOrDefaultAsync_WithResultSpec_ReturnsResult()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminIdByEmailSpec(admin.Email);

            // Act
            var result = await repo.FirstOrDefaultAsync(spec);

            // Assert
            Assert.That(result, Is.EqualTo(admin.Id));
        }

        [Test]
        public async Task ListAsync_WithoutSpec_ReturnsAllEntities()
        {
            // Arrange
            var admins = new List<Admin> { CreateAdmin("admin1@example.com"), CreateAdmin("admin2@example.com") };
            Context.Admins.AddRange(admins);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);

            // Act
            var result = await repo.ListAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task ListAsync_WithSpec_ReturnsFilteredEntities()
        {
            // Arrange
            var admin1 = CreateAdmin("admin1@example.com");
            var admin2 = CreateAdmin("admin2@example.com");
            Context.Admins.AddRange([admin1, admin2]);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminByEmailSpec(admin1.Email);

            // Act
            var result = await repo.ListAsync(spec);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(admin1.Id));
        }

        [Test]
        public async Task ListAsync_WithResultSpec_ReturnsResults()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminIdByEmailSpec(admin.Email);

            // Act
            var result = await repo.ListAsync(spec);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(admin.Id));
        }

        [Test]
        public async Task DeleteAsync_RemovesEntity()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);

            // Act
            await repo.DeleteAsync(admin);
            Context.SaveChanges();

            // Assert
            var result = Context.Admins.Find(admin.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteRangeAsync_WithEntities_RemovesEntities()
        {
            // Arrange
            var admins = new List<Admin> { CreateAdmin("admin1@example.com"), CreateAdmin("admin2@example.com") };
            Context.Admins.AddRange(admins);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);

            // Act
            await repo.DeleteRangeAsync(admins);
            Context.SaveChanges();

            // Assert
            var results = Context.Admins.Where(a => admins.Select(x => x.Id).Contains(a.Id)).ToList();
            Assert.That(results, Has.Count.EqualTo(0));
        }

        [Test]
        public async Task DeleteRangeAsync_WithSpec_RemovesMatchingEntities()
        {
            // Arrange
            var admin1 = CreateAdmin("admin1@example.com");
            var admin2 = CreateAdmin("admin2@example.com");
            Context.Admins.AddRange([admin1, admin2]);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminByEmailSpec(admin1.Email);

            // Act
            await repo.DeleteRangeAsync(spec);
            Context.SaveChanges();

            // Assert
            var result = Context.Admins.Find(admin1.Id);
            Assert.That(result, Is.Null);
            var remaining = Context.Admins.Find(admin2.Id);
            Assert.That(remaining, Is.Not.Null);
        }

        [Test]
        public async Task SingleOrDefaultAsync_WithSpec_ReturnsEntity()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminSingleByEmailSpec(admin.Email);

            // Act
            var result = await repo.SingleOrDefaultAsync(spec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(admin.Id));
        }

        [Test]
        public async Task SingleOrDefaultAsync_WithResultSpec_ReturnsResult()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminIdByEmailSpec(admin.Email);

            // Act
            var result = await repo.SingleOrDefaultAsync(spec);

            // Assert
            Assert.That(result, Is.EqualTo(admin.Id));
        }

        [Test]
        public async Task UpdateAsync_UpdatesEntity()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            admin.FirstName = "UpdatedFirstName";
            var repo = new Repository<Admin>(Context);

            // Act
            await repo.UpdateAsync(admin);
            Context.SaveChanges();

            // Assert
            var result = Context.Admins.Find(admin.Id);
            Assert.That(result!.FirstName, Is.EqualTo("UpdatedFirstName"));
        }

        [Test]
        public async Task UpdateRangeAsync_UpdatesEntities()
        {
            // Arrange
            var admins = new List<Admin> { CreateAdmin("admin1@example.com"), CreateAdmin("admin2@example.com") };
            Context.Admins.AddRange(admins);
            Context.SaveChanges();
            admins[0].FirstName = "Updated1";
            admins[1].FirstName = "Updated2";
            var repo = new Repository<Admin>(Context);

            // Act
            await repo.UpdateRangeAsync(admins);
            Context.SaveChanges();

            // Assert
            var result1 = Context.Admins.Find(admins[0].Id);
            var result2 = Context.Admins.Find(admins[1].Id);
            Assert.Multiple(() =>
            {
                Assert.That(result1!.FirstName, Is.EqualTo("Updated1"));
                Assert.That(result2!.FirstName, Is.EqualTo("Updated2"));
            });
        }

        [Test]
        public async Task AsAsyncEnumerable_WithSpec_ReturnsAsyncEnumerable()
        {
            // Arrange
            var admin = CreateAdmin();
            Context.Admins.Add(admin);
            Context.SaveChanges();
            var repo = new Repository<Admin>(Context);
            var spec = new AdminByEmailSpec(admin.Email);

            // Act
            var enumerable = repo.AsAsyncEnumerable(spec);
            var list = new List<Admin>();
            await foreach (var item in enumerable)
            {
                list.Add(item);
            }

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(list, Has.Count.EqualTo(1));
                Assert.That(list[0].Id, Is.EqualTo(admin.Id));
            });
        }
    }
}
