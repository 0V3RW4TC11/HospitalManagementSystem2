using Abstractions;
using Handlers;
using Commands.Patient;
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
    internal class PatientCommandHandlerTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        private RepositoryDbContext Context => _serviceProvider.GetRequiredService<RepositoryDbContext>();
        private IUnitOfWork UnitOfWork => _serviceProvider.GetRequiredService<IUnitOfWork>();
        private UserManager<IdentityUser> UserManager => _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

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

            _serviceProvider = services.BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
            context.Database.EnsureCreated();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            roleManager.CreateAsync(new IdentityRole(Constants.AuthRoles.Patient)).Wait();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        public async Task HandleCreatePatientCommand_ValidRequest_AddsPatientAndCreatesIdentity()
        {
            // Arrange
            var context = Context;
            var userManager = UserManager;
            var handler = new PatientCommandHandler(UnitOfWork);
            var patientData = new PatientData(
                "Mr",
                "John",
                "Doe",
                "Male",
                "123 Main St",
                "1234567890",
                "john@example.com",
                Constants.BloodType.AbNegative,
                new DateOnly(1980, 1, 1));
            var password = "SecurePassword123!";
            var command = new CreatePatientCommand(patientData, password);

            // Act
            await handler.Handle(command);

            // Assert
            // Patient exists with correct data
            var patient = await context.Patients.AsNoTracking().SingleAsync(p => p.Email == patientData.Email);
            Assert.That(patient, Is.Not.Null, "Patient");
            Assert.Multiple(() =>
            {
                Assert.That(patient.Title, Is.EqualTo(patientData.Title), "Title");
                Assert.That(patient.FirstName, Is.EqualTo(patientData.FirstName), "FirstName");
                Assert.That(patient.LastName, Is.EqualTo(patientData.LastName), "LastName");
                Assert.That(patient.Gender, Is.EqualTo(patientData.Gender), "Gender");
                Assert.That(patient.Address, Is.EqualTo(patientData.Address), "Address");
                Assert.That(patient.Phone, Is.EqualTo(patientData.Phone), "Phone");
                Assert.That(patient.Email, Is.EqualTo(patientData.Email), "Email");
                Assert.That(patient.BloodType, Is.EqualTo(patientData.BloodType), "BloodType");
                Assert.That(patient.DateOfBirth, Is.EqualTo(patientData.DateOfBirth), "DateOfBirth");
            });

            // IdentityUser exists with correct data
            var user = await context.Users.SingleAsync();
            Assert.That(user, Is.Not.Null, "IdentityUser");
            Assert.That(await userManager.CheckPasswordAsync(user, password), Is.True, "Password check");

            // IdentityUserRole exists with correct data
            var userRole = await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id);
            Assert.That(userRole, Is.Not.Null, "IdentityUserRole");
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == userRole.RoleId);
            Assert.That(role, Is.Not.Null, "IdentityRole");
            Assert.That(role.Name, Is.EqualTo(Constants.AuthRoles.Patient), "Role name");

            // Claim exists with correct data
            var claims = await userManager.GetClaimsAsync(user);
            Assert.That(claims, Is.Not.Null, "Claims");
            var hmsIdClaim = claims.FirstOrDefault(c => c.Type == Persistence.AppConstants.ClaimConstants.HmsUserId);
            Assert.That(hmsIdClaim, Is.Not.Null, "HmsUserId claim");
            Assert.That(Guid.Parse(hmsIdClaim.Value), Is.EqualTo(patient.Id), "HmsUserId claim value");
        }

        [Test]
        public async Task HandleDeletePatientCommand_ValidId_DeletesPatientAndIdentity()
        {
            // Arrange
            var context = Context;
            var handler = new PatientCommandHandler(UnitOfWork);
            var patientData = new PatientData(
                "Mr",
                "John",
                "Doe",
                "Male",
                "123 Main St",
                "1234567890",
                "john@example.com",
                Constants.BloodType.AbNegative,
                new DateOnly(1980, 1, 1));
            var password = "SecurePassword123!";
            var createCommand = new CreatePatientCommand(patientData, password);
            await handler.Handle(createCommand);
            var patient = await context.Patients.AsNoTracking().SingleAsync(p => p.Email == patientData.Email);
            var deleteCommand = new DeletePatientCommand(patient.Id);

            context.ChangeTracker.Clear();

            // Act
            await handler.Handle(deleteCommand);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(context.Patients, Is.Empty);
                Assert.That(context.Users, Is.Empty);
                Assert.That(context.UserRoles, Is.Empty);
                Assert.That(context.UserClaims, Is.Empty);
            });
        }

        [Test]
        public async Task HandleUpdatePatientCommand_ValidRequest_UpdatesPatient()
        {
            // Arrange
            var context = Context;
            var handler = new PatientCommandHandler(UnitOfWork);
            var patientData1 = new PatientData(
                "Mr",
                "John",
                "Doe",
                "Male",
                "123 Main St",
                "1234567890",
                "john@example.com",
                Constants.BloodType.AbNegative,
                new DateOnly(1980, 1, 1));
            var password = "SecurePassword123!";
            var createCommand = new CreatePatientCommand(patientData1, password);
            await handler.Handle(createCommand);
            var patient = await context.Patients.AsNoTracking().SingleAsync(p => p.Email == patientData1.Email);
            var patientData2 = new PatientData(
                "Dr",
                "Jane",
                "Smith",
                "Female",
                "456 Second St",
                "9876543210",
                "jane@example.com",
                Constants.BloodType.OPositive,
                new DateOnly(1985, 5, 15));
            var updateCommand = new UpdatePatientCommand(patient.Id, patientData2);

            context.ChangeTracker.Clear();

            // Act
            await handler.Handle(updateCommand);

            // Assert
            var updatedPatient = await context.Patients.AsNoTracking().SingleAsync(p => p.Id == patient.Id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedPatient.Title, Is.EqualTo(patientData2.Title));
                Assert.That(updatedPatient.FirstName, Is.EqualTo(patientData2.FirstName));
                Assert.That(updatedPatient.LastName, Is.EqualTo(patientData2.LastName));
                Assert.That(updatedPatient.Gender, Is.EqualTo(patientData2.Gender));
                Assert.That(updatedPatient.Address, Is.EqualTo(patientData2.Address));
                Assert.That(updatedPatient.Phone, Is.EqualTo(patientData2.Phone));
                Assert.That(updatedPatient.Email, Is.EqualTo(patientData2.Email));
                Assert.That(updatedPatient.BloodType, Is.EqualTo(patientData2.BloodType));
                Assert.That(updatedPatient.DateOfBirth, Is.EqualTo(patientData2.DateOfBirth));
            });
        }
    }
}
