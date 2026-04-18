using Abstractions;
using Commands.Doctor;
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
    internal class DoctorCommandHandlerTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;
        private string _doctorRoleId = string.Empty;

        private HmsDbContext Context => _serviceProvider.GetRequiredService<HmsDbContext>();
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

            services.AddDbContext<HmsDbContext>(options =>
                options.UseSqlite(_connection));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<HmsDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<StaffService, AspIdentityStaffService>();

            _serviceProvider = services.BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HmsDbContext>();
            context.Database.EnsureCreated();

            var identityRole = new IdentityRole(Constants.AuthRoles.Doctor);
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            roleManager.CreateAsync(identityRole).Wait();
            _doctorRoleId = identityRole.Id;
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        private DoctorData CreateDoctorData(string firstName, string lastName, string email, IEnumerable<Guid>? specializationIds = null)
        {
            specializationIds ??= new List<Guid>();

            return new DoctorData(
                firstName,
                lastName,
                "Male",
                "123 Medical St",
                "1234567890",
                email,
                new DateOnly(1980, 1, 1),
                specializationIds);
        }

        private List<Guid> CreateSpecializations(params string[] names)
        {
            var specializationIds = new List<Guid>();

            foreach (var name in names)
            {
                var specialization = new Domain.Entities.Specialization { Name = name };
                Context.Specializations.Add(specialization);
                Context.SaveChanges();
                specializationIds.Add(specialization.Id);
            }

            return specializationIds;
        }

        [Test]
        public async Task HandleCreateDoctorCommand_ValidRequest_Creates_Doctor_IdentityUser_DoctorSpecializations()
        {
            // Arrange
            var context = Context;
            var userManager = UserManager;
            var handler = new DoctorCommandHandler(UnitOfWork, StaffService);

            var specializationIds = CreateSpecializations("Cardiology", "Neurology");
            var doctorData = CreateDoctorData("John", "Doe", "john.doe@hospital.com", specializationIds);
            var password = "SecurePassword123!";
            var command = new CreateDoctorCommand(doctorData, password);

            context.ChangeTracker.Clear(); // clears dangling entity tracking references due to non normal use of DbContext

            // Act
            await handler.Handle(command);

            // Assert

            // Doctor exists with correct data
            var doctor = context.Doctors.AsNoTracking().SingleOrDefault();
            Assert.That(doctor, Is.Not.Null, "1");
            Assert.Multiple(() =>
            {
                Assert.That(doctor.FirstName, Is.EqualTo(doctorData.FirstName), "1.1");
                Assert.That(doctor.LastName, Is.EqualTo(doctorData.LastName), "1.2");
                Assert.That(doctor.Gender, Is.EqualTo(doctorData.Gender), "1.3");
                Assert.That(doctor.Address, Is.EqualTo(doctorData.Address), "1.4");
                Assert.That(doctor.Phone, Is.EqualTo(doctorData.Phone), "1.5");
                Assert.That(doctor.Email, Is.EqualTo(doctorData.Email), "1.6");
                Assert.That(doctor.DateOfBirth, Is.EqualTo(doctorData.DateOfBirth), "1.7");
            });

            // DoctorSpecializations exist
            var doctorSpecializations = await context.DoctorSpecializations
                .AsNoTracking()
                .Where(ds => ds.DoctorId == doctor.Id)
                .ToListAsync();
            Assert.That(doctorSpecializations, Has.Count.EqualTo(specializationIds.Count), "2");

            // IdentityUser exists
            var user = context.Users.SingleOrDefault(u => u.UserName == "john.doe@" + Constants.DomainNames.Organization);
            Assert.That(user, Is.Not.Null, "3");
            Assert.That(await userManager.CheckPasswordAsync(user, password), Is.True, "3.1");

            // IdentityUserRole exists with correct data
            var userRole = context.UserRoles.SingleOrDefault(ur => ur.UserId == user.Id);
            Assert.That(userRole, Is.Not.Null, "4");
            Assert.That(userRole.RoleId, Is.EqualTo(_doctorRoleId), "4.1");

            // Claim exists with correct data
            var claims = await userManager.GetClaimsAsync(user);
            Assert.That(claims, Is.Not.Null, "5");
            var hmsIdClaim = claims.SingleOrDefault(c => c.Type == Persistence.AppConstants.ClaimConstants.HmsUserId);
            Assert.That(hmsIdClaim, Is.Not.Null, "5.1");
            Assert.That(Guid.Parse(hmsIdClaim.Value), Is.EqualTo(doctor.Id), "5.2");
        }

        [Test]
        public async Task HandleDeleteDoctorCommand_ValidRequest_Deletes_Doctor_IdentityUser_DoctorSpecializations()
        {
            // Arrange
            var context = Context;
            var handler = new DoctorCommandHandler(UnitOfWork, StaffService);

            var specializationIds = CreateSpecializations("Orthopedics", "Rheumatology");
            var doctorData = CreateDoctorData("Jane", "Smith", "jane.smith@hospital.com", specializationIds);
            var password = "SecurePassword123!";
            var createCommand = new CreateDoctorCommand(doctorData, password);
            await handler.Handle(createCommand);
            var doctor = await context.Doctors.AsNoTracking().SingleAsync(d => d.Email == doctorData.Email);
            var deleteCommand = new DeleteDoctorCommand(doctor.Id);

            context.ChangeTracker.Clear(); // clears dangling entity tracking references due to non normal use of DbContext

            // Act
            await handler.Handle(deleteCommand);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(context.Doctors, Is.Empty, "1.1");
                Assert.That(context.Users, Is.Empty, "1.2");
                Assert.That(context.UserRoles, Is.Empty, "1.3");
                Assert.That(context.UserClaims, Is.Empty, "1.4");
                Assert.That(context.DoctorSpecializations, Is.Empty, "1.5");
            });
        }

        [Test]
        public async Task HandleUpdateDoctorCommand_ValidRequest_Updates_Doctor_DoctorSpecializations()
        {
            // Arrange
            var context = Context;
            var handler = new DoctorCommandHandler(UnitOfWork, StaffService);

            var specializationIds1 = CreateSpecializations("Pediatrics");
            var doctorData1 = CreateDoctorData("Robert", "Johnson", "robert.johnson@hospital.com", specializationIds1);
            var password = "SecurePassword123!";
            var createCommand = new CreateDoctorCommand(doctorData1, password);
            await handler.Handle(createCommand, CancellationToken.None);
            var doctor = await context.Doctors.AsNoTracking().SingleAsync(d => d.Email == doctorData1.Email);

            var specializationIds2 = CreateSpecializations("Psychiatry", "Dermatology");
            var doctorData2 = CreateDoctorData("Robert", "Johnson", "robert.johnson.new@hospital.com", specializationIds2);
            var updateCommand = new UpdateDoctorCommand(doctor.Id, doctorData2);

            context.ChangeTracker.Clear(); // clears dangling entity tracking references due to non normal use of DbContext

            // Act
            await handler.Handle(updateCommand);

            // Assert
            var updatedDoctor = await context.Doctors.AsNoTracking().SingleAsync(d => d.Id == doctor.Id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedDoctor.FirstName, Is.EqualTo(doctorData2.FirstName));
                Assert.That(updatedDoctor.LastName, Is.EqualTo(doctorData2.LastName));
                Assert.That(updatedDoctor.Gender, Is.EqualTo(doctorData2.Gender));
                Assert.That(updatedDoctor.Address, Is.EqualTo(doctorData2.Address));
                Assert.That(updatedDoctor.Phone, Is.EqualTo(doctorData2.Phone));
                Assert.That(updatedDoctor.Email, Is.EqualTo(doctorData2.Email));
                Assert.That(updatedDoctor.DateOfBirth, Is.EqualTo(doctorData2.DateOfBirth));
            });

            // DoctorSpecializations are updated
            var doctorSpecializations = await context.DoctorSpecializations
                .AsNoTracking()
                .Where(ds => ds.DoctorId == doctor.Id)
                .ToListAsync();
            Assert.That(doctorSpecializations, Has.Count.EqualTo(specializationIds2.Count()));
        }
    }
}
