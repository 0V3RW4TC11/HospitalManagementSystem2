using Abstractions;
using Commands.Specialization;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Specialization;

namespace Validation.Tests.Specialization;

[TestFixture]
public class CreateSpecializationValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CreateSpecializationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new CreateSpecializationValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_NameIsNotUnique_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateSpecializationCommand("Existing Name");

        _unitOfWorkMock.Setup(u => u.Specializations.AnyAsync(It.IsAny<Specifications.Specialization.SpecializationByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("This name is already used by another Specialization.");
    }

    [Test]
    public async Task Validate_NameIsUnique_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateSpecializationCommand("Unique Name");

        _unitOfWorkMock.Setup(u => u.Specializations.AnyAsync(It.IsAny<Specifications.Specialization.SpecializationByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Validate_NameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateSpecializationCommand("");

        _unitOfWorkMock.Setup(u => u.Specializations.AnyAsync(It.IsAny<Specifications.Specialization.SpecializationByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name is required.");
    }
}