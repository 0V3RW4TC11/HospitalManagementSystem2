using Abstractions;
using Commands.Admin;
using Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;
using Validation.Behaviors;

namespace Tests.RequestPipeline
{
    [TestFixture]
    internal class AdminPipelineTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        private DbSet<Admin> GetAdmins()
        {
            var context = _serviceProvider.GetRequiredService<Persistence.HmsDbContext>();
            return context.Admins;
        }

        private ISender GetSender() => _serviceProvider.GetRequiredService<ISender>();

        private UserManager<IdentityUser> GetUserManager() => _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

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

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<HmsDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<StaffService, AspIdentityStaffService>();

            services.AddValidatorsFromAssembly(typeof(Validation.AssemblyReference).Assembly);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(global::Handlers.AssemblyReference).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            _serviceProvider = services.BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HmsDbContext>();
            context.Database.EnsureCreated();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            roleManager.CreateAsync(new IdentityRole(Constants.AuthRoles.Admin)).Wait();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        public async Task CreateAdminCommand_ValidCommand_ShouldPass()
        {
            // Arrange
            var command = new CreateAdminCommand(
                new AdminData("Mr", "John", "Doe", "Male", "123 Main St", "1234567890", "john@example.com", new DateOnly(1980, 1, 1)),
                "SecurePassword123!");

            // Act
            await GetSender().Send(command);

            // Assert
            Assert.That(GetAdmins().Count(), Is.EqualTo(1));
            Assert.That(GetUserManager().Users.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CreateAdminCommand_InvalidCommand_ShouldFail()
        {
            // Arrange
            var command = new CreateAdminCommand(
                new AdminData("Mr", "", "Doe", "Male", "123 Main St", "1234567890", "john@example.com", new DateOnly(1980, 1, 1)),
                "SecurePassword123!");

            // Assert
            Assert.ThrowsAsync<ValidationException>(async () => await GetSender().Send(command));
        }
    }
}
