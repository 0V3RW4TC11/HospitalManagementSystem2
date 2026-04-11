using Abstractions;
using Commands.Doctor;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Doctor;

namespace Validation.Tests.Doctor;

[TestFixture]
internal class UpdateDoctorValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Doctor>> _doctorRepoMock;
    private UpdateDoctorValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new();
        _doctorRepoMock = new();
        _unitOfWorkMock.Setup(u => u.Doctors).Returns(_doctorRepoMock.Object);
        _validator = new(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_EmailIsUsedByAnotherDoctor_ShouldHaveValidationError()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var anotherDoctorId = Guid.NewGuid();
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
        var command = new UpdateDoctorCommand(doctorId, doctorData);

        _unitOfWorkMock.Setup(u => u.Doctors.SingleOrDefaultAsync(It.IsAny<Specifications.Doctor.DoctorIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(anotherDoctorId); // Email used by another doctor
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Doctor.");
    }

    [Test]
    public async Task Validate_InvalidDoctorData_ShouldHaveValidationErrors()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctorData = new DoctorData("", "Doe", "Male", "Address", "", "", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
        var command = new UpdateDoctorCommand(doctorId, doctorData);

        _unitOfWorkMock.Setup(u => u.Doctors.SingleOrDefaultAsync(It.IsAny<Specifications.Doctor.DoctorIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.Data.Phone);
        result.ShouldHaveValidationErrorFor(x => x.Data.Email);
    }

    [Test]
    public async Task Validate_EmailIsUnique_ShouldNotHaveValidationError()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
        var command = new UpdateDoctorCommand(doctorId, doctorData);

        _unitOfWorkMock.Setup(u => u.Doctors.SingleOrDefaultAsync(It.IsAny<Specifications.Doctor.DoctorIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty); // Email not used
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
    }

    [Test]
    public async Task Validate_EmailIsSameAsCurrentDoctor_ShouldNotHaveValidationError()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
        var command = new UpdateDoctorCommand(doctorId, doctorData);

        _unitOfWorkMock.Setup(u => u.Doctors.SingleOrDefaultAsync(It.IsAny<Specifications.Doctor.DoctorIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctorId); // Email used by same doctor
        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
    }
}