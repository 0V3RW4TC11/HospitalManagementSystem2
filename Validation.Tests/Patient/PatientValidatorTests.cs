using Commands.Patient;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Validation.Patient;

namespace Validation.Tests.Patient
{
    [TestFixture]
    internal class PatientValidatorTests
    {
        private readonly PatientValidator _validator = new();

        [Test]
        public void Validate_FirstNameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new PatientData("", "", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));

            // Act
            var result = _validator.TestValidate(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("First name is required.");
        }

        [Test]
        public void Validate_GenderIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new PatientData("", "FirstName", "", "", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));

            // Act
            var result = _validator.TestValidate(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Gender).WithErrorMessage("Gender is required.");
        }

        [Test]
        public void Validate_EmailIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new PatientData("", "FirstName", "", "Male", "", "", "", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));

            // Act
            var result = _validator.TestValidate(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is required.");
        }

        [Test]
        public void Validate_ValidPatientData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var data = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));

            // Act
            var result = _validator.TestValidate(data);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
