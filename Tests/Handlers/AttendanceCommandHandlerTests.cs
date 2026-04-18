using Abstractions;
using Commands.Attendance;
using Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;
using Entities;

namespace Tests.Handlers
{
    [TestFixture]
    internal class AttendanceCommandHandlerTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        private HmsDbContext Context => _serviceProvider.GetRequiredService<HmsDbContext>();
        private IUnitOfWork UnitOfWork => _serviceProvider.GetRequiredService<IUnitOfWork>();

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
        public async Task HandleCreateAttendanceCommand_ValidRequest_CreatesAttendance()
        {
            // Arrange
            var context = Context;
            var handler = new AttendanceCommandHandler(UnitOfWork);
            var patient = CreatePatient();
            var doctor = CreateDoctor();
            context.Patients.Add(patient);
            context.Doctors.Add(doctor);
            context.SaveChanges();

            var data = new AttendanceData(
                patient.Id,
                doctor.Id,
                DateTime.Now,
                "Example diagnosis",
                "Example remarks",
                "Example therapy");
            var command = new CreateAttendanceCommand(data);

            // Act
            await handler.Handle(command);

            // Assert
            var attendance = await context.Attendances.AsNoTracking().SingleOrDefaultAsync();
            Assert.That(attendance, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(attendance.PatientId, Is.EqualTo(data.PatientId));
                Assert.That(attendance.DoctorId, Is.EqualTo(data.DoctorId));
                Assert.That(attendance.DateTime, Is.EqualTo(data.DateTime));
                Assert.That(attendance.Diagnosis, Is.EqualTo(data.Diagnosis));
                Assert.That(attendance.Remarks, Is.EqualTo(data.Remarks));
                Assert.That(attendance.Therapy, Is.EqualTo(data.Therapy));
            });
        }

        [Test]
        public async Task HandleDeleteAttendanceCommand_ValidRequest_DeletesAttendance()
        {
            // Arrange
            var context = Context;
            var handler = new AttendanceCommandHandler(UnitOfWork);
            var patient = CreatePatient();
            var doctor = CreateDoctor();
            context.Patients.Add(patient);
            context.Doctors.Add(doctor);
            context.SaveChanges();

            var data = new AttendanceData(
                patient.Id,
                doctor.Id,
                DateTime.Now,
                "Example diagnosis",
                "Example remarks",
                "Example therapy");

            var createCommand = new CreateAttendanceCommand(data);
            await handler.Handle(createCommand);

            var attendance = await context.Attendances.AsNoTracking().SingleAsync();
            var deleteCommand = new DeleteAttendanceCommand(attendance.Id);

            // Act
            await handler.Handle(deleteCommand);

            // Assert
            Assert.That(context.Attendances.Any(), Is.False);
        }

        [Test]
        public async Task HandleUpdateAttendanceCommand_ValidRequest_UpdatesAttendance()
        {
            // Arrange
            var context = Context;
            var handler = new AttendanceCommandHandler(UnitOfWork);
            var patient = CreatePatient();
            var doctor = CreateDoctor();
            context.Patients.Add(patient);
            context.Doctors.Add(doctor);
            context.SaveChanges();

            var data = new AttendanceData(
                patient.Id,
                doctor.Id,
                DateTime.Now,
                "Example diagnosis",
                "Example remarks",
                "Example therapy");

            var createCommand = new CreateAttendanceCommand(data);
            await handler.Handle(createCommand);

            var updatedData = new AttendanceData(
                patient.Id,
                doctor.Id,
                DateTime.Now,
                "Updated diagnosis",
                "Updated remarks",
                "Updated therapy");

            var attendance = await Context.Attendances.AsNoTracking().SingleAsync();
            var updateCommand = new UpdateAttendanceCommand(attendance.Id, updatedData);

            Context.ChangeTracker.Clear();

            // Act
            await handler.Handle(updateCommand);

            // Assert
            var updatedAttendance = await Context.Attendances.AsNoTracking().SingleAsync();
            Assert.Multiple(() =>
            {
                Assert.That(updatedAttendance.PatientId, Is.EqualTo(updatedData.PatientId));
                Assert.That(updatedAttendance.DoctorId, Is.EqualTo(updatedData.DoctorId));
                Assert.That(updatedAttendance.DateTime, Is.EqualTo(updatedData.DateTime));
                Assert.That(updatedAttendance.Diagnosis, Is.EqualTo(updatedData.Diagnosis));
                Assert.That(updatedAttendance.Remarks, Is.EqualTo(updatedData.Remarks));
                Assert.That(updatedAttendance.Therapy, Is.EqualTo(updatedData.Therapy));
            });
        }

        private static Patient CreatePatient() => new()
        {
            FirstName = "Joe",
            Gender = "Male",
            Email = "joe@example.com",
            BloodType = Constants.BloodType.APositive,
            DateOfBirth = new DateOnly(1980, 1, 1)
        };

        private static Doctor CreateDoctor() => new()
        {
            FirstName = "Fred",
            LastName = "Smith",
            Gender = "Male",
            Address = "123 Main St",
            Phone = "1231231234",
            Email = "f.smith@example.com",
            DateOfBirth = new DateOnly(1980, 1, 1)
        };
    }
}
