using Abstractions;
using Commands.Doctor;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Doctor;

namespace Validation.Tests.Doctor;

[TestFixture]
public class DeleteDoctorValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private DeleteDoctorValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new DeleteDoctorValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_IdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteDoctorCommand(Guid.Empty);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
    }

    [Test]
    public async Task Validate_DoctorDoesNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteDoctorCommand(id);

        _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Doctor with this Id does not exist.");
    }

    [Test]
    public async Task Validate_DoctorExists_ShouldNotHaveValidationError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteDoctorCommand(id);

        _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Doctor>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}