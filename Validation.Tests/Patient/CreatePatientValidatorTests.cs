using Abstractions;
using Commands.Patient;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Patient;

namespace Validation.Tests.Patient
{
    [TestFixture]
    internal class CreatePatientValidatorTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IRepository<Domain.Entities.Patient>> _patientsMock;
        private CreatePatientValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new();
            _patientsMock = new();
            _unitOfWorkMock.Setup(u => u.Patients).Returns(_patientsMock.Object);
            _validator = new(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task Validate_PasswordIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new CreatePatientCommand(patientData, "");

            _patientsMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Patient.PatientByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password is required.");
        }

        [Test]
        public async Task Validate_EmailIsNotUnique_ShouldHaveValidationError()
        {
            // Arrange
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new CreatePatientCommand(patientData, "password");

            _patientsMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Patient.PatientByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Patient");
        }

        [Test]
        public async Task Validate_ValidCommand_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new CreatePatientCommand(patientData, "password");

            _patientsMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Patient.PatientByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
