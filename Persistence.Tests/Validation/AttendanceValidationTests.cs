using Abstractions;
using Commands.Attendance;
using FluentValidation.TestHelper;
using Moq;
using Validation.Attendance;

namespace Tests.Validation
{
    [TestFixture]
    internal class AttendanceValidationTests
    {
        private static void CreateUowDoctorPatientMocks(
            out Mock<IUnitOfWork> uowMock,
            out Mock<IRepository<Domain.Entities.Doctor>> doctorRepoMock,
            out Mock<IRepository<Domain.Entities.Patient>> patientRepoMock)
        {
            uowMock = new();
            doctorRepoMock = new();
            patientRepoMock = new();
            uowMock.Setup(u => u.Doctors).Returns(doctorRepoMock.Object);
            uowMock.Setup(u => u.Patients).Returns(patientRepoMock.Object);
        }

        private static void CreateUowDoctorPatientAttendanceMocks(
            out Mock<IUnitOfWork> uowMock,
            out Mock<IRepository<Domain.Entities.Doctor>> doctorRepoMock,
            out Mock<IRepository<Domain.Entities.Patient>> patientRepoMock,
            out Mock<IRepository<Domain.Entities.Attendance>> attendanceRepoMock)
        {
            CreateUowDoctorPatientMocks(out uowMock, out doctorRepoMock, out patientRepoMock);
            attendanceRepoMock = new();
            uowMock.Setup(u => u.Attendances).Returns(attendanceRepoMock.Object);
        }

        [Test]
        public async Task AttendanceValidator_DiagnosisIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "", "Remarks", "Therapy");
            CreateUowDoctorPatientMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new AttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Diagnosis).WithErrorMessage("Diagnosis is required.");
        }

        [Test]
        public async Task AttendanceValidator_RemarksIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "", "Therapy");
            CreateUowDoctorPatientMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new AttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Remarks).WithErrorMessage("Remarks is required.");
        }

        [Test]
        public async Task AttendanceValidator_TherapyIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Remarks", "");
            CreateUowDoctorPatientMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new AttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Therapy).WithErrorMessage("Therapy is required.");
        }

        [Test]
        public async Task AttendanceValidator_DateTimeIsInFuture_ShouldHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), "Diagnosis", "Therapy", "Remarks");
            CreateUowDoctorPatientMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new AttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DateTime).WithErrorMessage("Date cannot be in the future.");
        }

        [Test]
        public async Task AttendanceValidator_DoctorDoesNotExist_ShouldHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
            CreateUowDoctorPatientMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new AttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DoctorId).WithErrorMessage("Doctor with this Id does not exist.");
        }

        [Test]
        public async Task AttendanceValidator_PatientDoesNotExist_ShouldHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
            CreateUowDoctorPatientMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var validator = new AttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PatientId).WithErrorMessage("Patient with this Id does not exist.");
        }

        [Test]
        public async Task AttendanceValidator_ValidData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
            CreateUowDoctorPatientMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new AttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(data);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task CreateValidator_AttendanceAlreadyExists_ShouldHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
            var command = new CreateAttendanceCommand(data);
            CreateUowDoctorPatientAttendanceMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock, out var attendanceRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Attendance()); // Exists
            var validator = new CreateAttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data).WithErrorMessage("An Attendance with similar details already exists.");
        }

        [Test]
        public async Task CreateValidator_AttendanceDoesNotExist_ShouldNotHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
            var command = new CreateAttendanceCommand(data);
            CreateUowDoctorPatientAttendanceMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock, out var attendanceRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Attendance?)null); // Does not exist
            var validator = new CreateAttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Data);
        }

        [Test]
        public async Task CreateValidator_InvalidData_ShouldHaveValidationErrors()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), "", "", "");
            var command = new CreateAttendanceCommand(data);
            CreateUowDoctorPatientAttendanceMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock, out var attendanceRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Attendance?)null);
            var validator = new CreateAttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.DateTime);
            result.ShouldHaveValidationErrorFor(x => x.Data.Diagnosis);
            result.ShouldHaveValidationErrorFor(x => x.Data.Therapy);
            result.ShouldHaveValidationErrorFor(x => x.Data.Remarks);
        }

        [Test]
        public async Task UpdateValidator_AttendanceAlreadyExistsWithDifferentId_ShouldHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
            var command = new UpdateAttendanceCommand(Guid.NewGuid(), data);
            CreateUowDoctorPatientAttendanceMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock, out var attendanceRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Attendance { Id = Guid.NewGuid() }); // Different id
            var validator = new UpdateAttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("An Attendance with similar details already exists.");
        }

        [Test]
        public async Task UpdateValidator_AttendanceExistsWithSameId_ShouldNotHaveValidationError()
        {
            // Arrange
            var attendanceId = Guid.NewGuid();
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
            var command = new UpdateAttendanceCommand(attendanceId, data);
            CreateUowDoctorPatientAttendanceMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock, out var attendanceRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Attendance { Id = attendanceId }); // Same id
            var validator = new UpdateAttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x);
        }

        [Test]
        public async Task UpdateValidator_NoSimilarAttendance_ShouldNotHaveValidationError()
        {
            // Arrange
            var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
            var command = new UpdateAttendanceCommand(Guid.NewGuid(), data);
            CreateUowDoctorPatientAttendanceMocks(out var uowMock, out var doctorRepoMock, out var patientRepoMock, out var attendanceRepoMock);
            doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Attendance?)null); // No similar
            var validator = new UpdateAttendanceValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
