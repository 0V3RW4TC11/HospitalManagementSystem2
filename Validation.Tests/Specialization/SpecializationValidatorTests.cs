using Commands.Specialization;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Validation.Specialization;

namespace Validation.Tests.Specialization;

[TestFixture]
internal class SpecializationValidatorTests
{
    private SpecializationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new SpecializationValidator();
    }

    [Test]
    public void Validate_NameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateSpecializationCommand("");

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name is required.");
    }

    [Test]
    public void Validate_NameIsNotEmpty_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateSpecializationCommand("Cardiology");

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}