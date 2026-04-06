using Abstractions;
using Commands.Attendance;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Attendance;

namespace Validation.Tests.Attendance;

[TestFixture]
public class CreateAttendanceValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Doctor>> _doctorRepoMock;
    private Mock<IRepository<Domain.Entities.Patient>> _patientRepoMock;
    private Mock<IRepository<Domain.Entities.Attendance>> _attendanceRepoMock;
    private CreateAttendanceValidator _validator;

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
        _validator = new CreateAttendanceValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_AttendanceAlreadyExists_ShouldHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
        var command = new CreateAttendanceCommand(data);

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Attendance()); // Exists

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data).WithErrorMessage("An Attendance with similar details already exists.");
    }

    [Test]
    public async Task Validate_AttendanceDoesNotExist_ShouldNotHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");
        var command = new CreateAttendanceCommand(data);

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Attendance?)null); // Does not exist

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Data);
    }

    [Test]
    public async Task Validate_InvalidData_ShouldHaveValidationErrors()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), "", "", "");
        var command = new CreateAttendanceCommand(data);

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _attendanceRepoMock.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Specifications.Attendance.AttendanceByDoctorPatientDateRangeSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Attendance?)null);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.DateTime);
        result.ShouldHaveValidationErrorFor(x => x.Data.Diagnosis);
        result.ShouldHaveValidationErrorFor(x => x.Data.Therapy);
        result.ShouldHaveValidationErrorFor(x => x.Data.Remarks);
    }
}