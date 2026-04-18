using Abstractions;
using Commands.Admin;
using Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;

namespace Tests.Handlers
{
    [TestFixture]
    internal class AdminCommandHandlerTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        private RepositoryDbContext Context => _serviceProvider.GetRequiredService<RepositoryDbContext>();
        private IUnitOfWork UnitOfWork => _serviceProvider.GetRequiredService<IUnitOfWork>();
        private UserManager<IdentityUser> UserManager => _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        private StaffService StaffService => _serviceProvider.GetRequiredService<StaffService>();

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

            services.AddDbContext<RepositoryDbContext>(options =>
                options.UseSqlite(_connection));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<RepositoryDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<StaffService, AspIdentityStaffService>();

            _serviceProvider = services.BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
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
        public async Task HandleCreateAdminCommand_ValidRequest_AddsAdminAndCreatesIdentity()
        {
            // Arrange
            var context = Context;
            var userManager = UserManager;
            var handler = new AdminCommandHandler(UnitOfWork, StaffService);
            var adminData = new AdminData("Mr", "John", "Doe", "Male", "123 Main St", "1234567890", "john@example.com", new DateOnly(1980,1, 1));
            var password = "SecurePassword123!";
            var command = new CreateAdminCommand(adminData, password);

            // Act
            await handler.Handle(command);

            // Assert

            // Admin exists with correct data
            var admin = await context.Admins.AsNoTracking().SingleAsync(a => a.Email == adminData.Email);
            Assert.That(admin, Is.Not.Null, "Admin");
            Assert.Multiple(() =>
            {
                Assert.That(admin.Title, Is.EqualTo(adminData.Title), "Title");
                Assert.That(admin.FirstName, Is.EqualTo(adminData.FirstName), "FirstName");
                Assert.That(admin.LastName, Is.EqualTo(adminData.LastName), "LastName");
                Assert.That(admin.Gender, Is.EqualTo(adminData.Gender), "Gender");
                Assert.That(admin.Address, Is.EqualTo(adminData.Address), "Address");
                Assert.That(admin.Phone, Is.EqualTo(adminData.Phone), "Phone");
                Assert.That(admin.Email, Is.EqualTo(adminData.Email), "Email");
                Assert.That(admin.DateOfBirth, Is.EqualTo(adminData.DateOfBirth), "DateOfBirth");
            });

            // IdentityUser exists with correct data
            var user = await context.Users.SingleAsync(u => u.UserName == "john.doe@sjog.org.au");
            Assert.That(user, Is.Not.Null, "IdentityUser");
            Assert.That(await userManager.CheckPasswordAsync(user, password), Is.True, "Password check");

            // IdentityUserRole exists with correct data
            var userRole = await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id);
            Assert.That(userRole, Is.Not.Null, "IdentityUserRole");
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == userRole.RoleId);
            Assert.That(role, Is.Not.Null, "IdentityRole");
            Assert.That(role.Name, Is.EqualTo(Constants.AuthRoles.Admin), "Role name");

            // Claim exists with correct data
            var claims = await userManager.GetClaimsAsync(user);
            Assert.That(claims, Is.Not.Null, "Claims");
            var hmsIdClaim = claims.FirstOrDefault(c => c.Type == Persistence.AppConstants.ClaimConstants.HmsUserId);
            Assert.That(hmsIdClaim, Is.Not.Null, "HmsUserId claim");
            Assert.That(Guid.Parse(hmsIdClaim.Value), Is.EqualTo(admin.Id), "HmsUserId claim value");
        }

        [Test]
        public async Task HandleDeleteAdminCommand_ValidId_DeletesAdminAndIdentity()
        {
            // Arrange
            var context = Context;
            var handler = new AdminCommandHandler(UnitOfWork, StaffService);
            var adminData = new AdminData("Mr", "John", "Doe", "Male", "123 Main St", "1234567890", "john@example.com", new DateOnly(1980, 1, 1));
            var password = "SecurePassword123!";
            var createCommand = new CreateAdminCommand(adminData, password);
            await handler.Handle(createCommand);
            var admin = await context.Admins.AsNoTracking().SingleAsync(a => a.Email == adminData.Email);
            var deleteCommand = new DeleteAdminCommand(admin.Id);

            context.ChangeTracker.Clear();

            // Act
            await handler.Handle(deleteCommand);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(context.Admins, Is.Empty);
                Assert.That(context.Users, Is.Empty);
                Assert.That(context.UserRoles, Is.Empty);
                Assert.That(context.UserClaims, Is.Empty);
            });
        }

        [Test]
        public async Task HandleUpdateAdminCommand_ValidRequest_UpdatesAdmin()
        {
            // Arrange
            var context = Context;
            var handler = new AdminCommandHandler(UnitOfWork, StaffService);
            var adminData1 = new AdminData("Mr", "John", "Doe", "Male", "123 Main St", "1234567890", "john@example.com", new DateOnly(1980, 1, 1));
            var password = "SecurePassword123!";
            var createCommand = new CreateAdminCommand(adminData1, password);
            await handler.Handle(createCommand, CancellationToken.None);
            var admin = await context.Admins.AsNoTracking().SingleAsync(a => a.Email == adminData1.Email);
            var adminData2 = new AdminData("Mr", "John", "Doe", "Male", "456 Second St", "1231231234", "john2@anotherexample.com", new DateOnly(1980, 1, 1));
            var updateCommand = new UpdateAdminCommand(admin.Id, adminData2);

            context.ChangeTracker.Clear();

            // Act
            await handler.Handle(updateCommand);

            // Assert
            var updatedAdmin = await context.Admins.AsNoTracking().SingleAsync(a => a.Id == admin.Id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedAdmin.Title, Is.EqualTo(adminData2.Title));
                Assert.That(updatedAdmin.FirstName, Is.EqualTo(adminData2.FirstName));
                Assert.That(updatedAdmin.LastName, Is.EqualTo(adminData2.LastName));
                Assert.That(updatedAdmin.Gender, Is.EqualTo(adminData2.Gender));
                Assert.That(updatedAdmin.Address, Is.EqualTo(adminData2.Address));
                Assert.That(updatedAdmin.Phone, Is.EqualTo(adminData2.Phone));
                Assert.That(updatedAdmin.Email, Is.EqualTo(adminData2.Email));
                Assert.That(updatedAdmin.DateOfBirth, Is.EqualTo(adminData2.DateOfBirth));
            });
        }
    }
}
