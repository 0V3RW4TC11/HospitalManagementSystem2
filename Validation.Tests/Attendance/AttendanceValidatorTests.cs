using Abstractions;
using Commands.Attendance;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Attendance;

namespace Validation.Tests.Attendance;

[TestFixture]
internal class AttendanceValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Doctor>> _doctorRepoMock;
    private Mock<IRepository<Domain.Entities.Patient>> _patientRepoMock;
    private AttendanceValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _doctorRepoMock = new Mock<IRepository<Domain.Entities.Doctor>>();
        _patientRepoMock = new Mock<IRepository<Domain.Entities.Patient>>();
        _unitOfWorkMock.Setup(u => u.Doctors).Returns(_doctorRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Patients).Returns(_patientRepoMock.Object);
        _validator = new AttendanceValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_DiagnosisIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "", "Remarks", "Therapy");

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(data);
        result.ShouldHaveValidationErrorFor(x => x.Diagnosis).WithErrorMessage("Diagnosis is required.");
    }

    [Test]
    public async Task Validate_RemarksIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "", "Therapy");

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(data);
        result.ShouldHaveValidationErrorFor(x => x.Remarks).WithErrorMessage("Remarks is required.");
    }

    [Test]
    public async Task Validate_TherapyIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Remarks", "");

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(data);
        result.ShouldHaveValidationErrorFor(x => x.Therapy).WithErrorMessage("Therapy is required.");
    }

    [Test]
    public async Task Validate_DateTimeIsInFuture_ShouldHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), "Diagnosis", "Therapy", "Remarks");

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(data);
        result.ShouldHaveValidationErrorFor(x => x.DateTime).WithErrorMessage("Date cannot be in the future.");
    }

    [Test]
    public async Task Validate_DoctorDoesNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(data);
        result.ShouldHaveValidationErrorFor(x => x.DoctorId).WithErrorMessage("Doctor with this Id does not exist.");
    }

    [Test]
    public async Task Validate_PatientDoesNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(data);
        result.ShouldHaveValidationErrorFor(x => x.PatientId).WithErrorMessage("Patient with this Id does not exist.");
    }

    [Test]
    public async Task Validate_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var data = new AttendanceData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "Diagnosis", "Therapy", "Remarks");

        _doctorRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(data);
        result.ShouldNotHaveAnyValidationErrors();
    }
}