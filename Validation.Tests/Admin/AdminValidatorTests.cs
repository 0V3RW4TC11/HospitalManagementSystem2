using Commands.Admin;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Validation.Admin;

namespace Validation.Tests.Admin;

[TestFixture]
internal class AdminValidatorTests
{
    private readonly AdminValidator _validator = new();

    [Test]
    public void Validate_FirstNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "", null, null, null, "1234567890", "test@example.com", null);

        // Act & Assert
        var result = _validator.TestValidate(adminData);
        result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("First name is required");
    }

    [Test]
    public void Validate_PhoneIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "", "test@example.com", null);

        // Act & Assert
        var result = _validator.TestValidate(adminData);
        result.ShouldHaveValidationErrorFor(x => x.Phone).WithErrorMessage("Phone number is required");
    }

    [Test]
    public void Validate_EmailIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "", null);

        // Act & Assert
        var result = _validator.TestValidate(adminData);
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is required");
    }

    [Test]
    public void Validate_ValidAdminData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);

        // Act & Assert
        var result = _validator.TestValidate(adminData);
        result.ShouldNotHaveAnyValidationErrors();
    }
}