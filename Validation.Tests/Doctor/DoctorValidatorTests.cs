using Commands.Doctor;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Validation.Doctor;

namespace Validation.Tests.Doctor;

[TestFixture]
public class DoctorValidatorTests
{
    private DoctorValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new DoctorValidator();
    }

    [Test]
    public async Task Validate_FirstNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var doctorData = new DoctorData("", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), [Guid.NewGuid()]);

        // Act & Assert
        var result = await _validator.TestValidateAsync(doctorData);
        result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("First name is required");
    }

    [Test]
    public async Task Validate_PhoneIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "", "test@example.com", new DateOnly(1980, 1, 1), [Guid.NewGuid()]);

        // Act & Assert
        var result = await _validator.TestValidateAsync(doctorData);
        result.ShouldHaveValidationErrorFor(x => x.Phone).WithErrorMessage("Phone number is required");
    }

    [Test]
    public async Task Validate_EmailIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "", new DateOnly(1980, 1, 1), [Guid.NewGuid()]);

        // Act & Assert
        var result = await _validator.TestValidateAsync(doctorData);
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is required");
    }

    [Test]
    public async Task Validate_SpecListIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), []);

        // Act & Assert
        var result = await _validator.TestValidateAsync(doctorData);
        result.ShouldHaveValidationErrorFor(x => x.SpecializationIds).WithErrorMessage("A Specialization is required.");
    }

    [Test]
    public async Task Validate_ValidDoctorData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), [Guid.NewGuid()]);

        // Act & Assert
        var result = await _validator.TestValidateAsync(doctorData);
        result.ShouldNotHaveAnyValidationErrors();
    }
}