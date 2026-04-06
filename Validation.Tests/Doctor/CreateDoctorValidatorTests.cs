using Abstractions;
using Commands.Doctor;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Doctor;

namespace Validation.Tests.Doctor;

[TestFixture]
public class CreateDoctorValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CreateDoctorValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new CreateDoctorValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_PasswordIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
        var command = new CreateDoctorCommand(doctorData, "");

        _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password is required.");
    }

    [Test]
    public async Task Validate_EmailIsNotUnique_ShouldHaveValidationError()
    {
        // Arrange
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
        var command = new CreateDoctorCommand(doctorData, "password");

        _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Doctor.");
    }

    [Test]
    public async Task Validate_EmailIsUnique_ShouldNotHaveValidationError()
    {
        // Arrange
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
        var command = new CreateDoctorCommand(doctorData, "password");

        _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
    }

    [Test]
    public async Task Validate_InvalidDoctorData_ShouldHaveValidationErrors()
    {
        // Arrange
        var doctorData = new DoctorData("", "Doe", "Male", "Address", "", "", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
        var command = new CreateDoctorCommand(doctorData, "password");

        _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.Data.Phone);
        result.ShouldHaveValidationErrorFor(x => x.Data.Email);
    }

    [Test]
    public async Task Validate_SpecializationIdsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid>());
        var command = new CreateDoctorCommand(doctorData, "password");

        _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.SpecializationIds).WithErrorMessage("Specializations are required.");
    }

    [Test]
    public async Task Validate_SpecializationIdsNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var specId = Guid.NewGuid();
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { specId });
        var command = new CreateDoctorCommand(doctorData, "password");

        _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0); // None exist

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.SpecializationIds).WithErrorMessage("One or more Specializations do not exist.");
    }
}