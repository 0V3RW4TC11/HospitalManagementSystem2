using Abstractions;
using Commands.Attendance;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Attendance;

namespace Validation.Tests.Attendance;

[TestFixture]
internal class UpdateAttendanceValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Doctor>> _doctorRepoMock;
    private Mock<IRepository<Domain.Entities.Patient>> _patientRepoMock;
    private Mock<IRepository<Domain.Entities.Attendance>> _attendanceRepoMock;
    private UpdateAttendanceValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _doctorRepoMock = new Mock<IRepository<Domain.Entities.Doctor>>();
        _patientRepoMock = new Mock<IRepository<Domain.Entities.Patient>>();
        _attendanceRepoMock = new Mock<IRepository<Domain.Entities.Attendance>>();
        _unitOfWorkMock.Setup(u => u.Doctors).Returns(_doctorRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Patients).Returns(_patientRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Attendances).Returns(_attendanceRepoMock.Object);
        _validator = new UpdateAttendanceValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_AttendanceAlreadyExistsWithDifferentId_ShouldHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
        var command = new UpdateAttendanceCommand(Guid.NewGuid(), data);

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Attendance { Id = Guid.NewGuid() }); // Different id

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("An Attendance with similar details already exists.");
    }

    [Test]
    public async Task Validate_AttendanceExistsWithSameId_ShouldNotHaveValidationError()
    {
        // Arrange
        var attendanceId = Guid.NewGuid();
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
        var command = new UpdateAttendanceCommand(attendanceId, data);

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Attendance { Id = attendanceId }); // Same id

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test]
    public async Task Validate_NoSimilarAttendance_ShouldNotHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
        var command = new UpdateAttendanceCommand(Guid.NewGuid(), data);

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Attendance?)null); // No similar

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x);
    }
}