using Abstractions;
using Commands.Patient;
using Entities;
using FluentValidation.TestHelper;
using Moq;
using Validation.Patient;

namespace Tests.Validators
{
    [TestFixture]
    internal class PatientValidationTests
    {
        private readonly PatientValidator validator = new();

        private static void CreateUowAndRepoMock(out Mock<IUnitOfWork> uowMock, out Mock<IRepository<Patient>> repoMock)
        {
            uowMock = new();
            repoMock = new();
            uowMock.Setup(u => u.Patients).Returns(repoMock.Object);
        }

        [Test]
        public void PatientValidator_FirstNameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new PatientData("", "", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));

            // Act
            var result = validator.TestValidate(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("First name is required.");
        }

        [Test]
        public void PatientValidator_GenderIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new PatientData("", "FirstName", "", "", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));

            // Act
            var result = validator.TestValidate(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Gender).WithErrorMessage("Gender is required.");
        }

        [Test]
        public void PatientValidator_EmailIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var data = new PatientData("", "FirstName", "", "Male", "", "", "", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));

            // Act
            var result = validator.TestValidate(data);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is required.");
        }

        [Test]
        public void PatientValidator_ValidPatientData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var data = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));

            // Act
            var result = validator.TestValidate(data);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task CreateValidator_PasswordIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new CreatePatientCommand(patientData, "");
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Patient.PatientByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var validator = new CreatePatientValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password is required.");
        }

        [Test]
        public async Task CreateValidator_EmailIsNotUnique_ShouldHaveValidationError()
        {
            // Arrange
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new CreatePatientCommand(patientData, "password");
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Patient.PatientByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new CreatePatientValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Patient");
        }

        [Test]
        public async Task CreateValidator_ValidCommand_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new CreatePatientCommand(patientData, "password");
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Patient.PatientByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var validator = new CreatePatientValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task UpdateValidator_EmailIsUsedByAnotherPatient_ShouldHaveValidationError()
        {
            // Arrange
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new UpdatePatientCommand(Guid.NewGuid(), patientData);
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            repoMock.Setup(p => p.SingleOrDefaultAsync(It.IsAny<Specifications.Patient.PatientIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid()); // Different ID
            var validator = new UpdatePatientValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Patient");
        }

        [Test]
        public async Task UpdateValidator_ValidCommand_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new UpdatePatientCommand(patientId, patientData);
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            repoMock.Setup(p => p.SingleOrDefaultAsync(It.IsAny<Specifications.Patient.PatientIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientId); // Same ID
            var validator = new UpdatePatientValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task UpdateValidator_EmailIsSameAsCurrent_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new UpdatePatientCommand(patientId, patientData);
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            repoMock.Setup(p => p.SingleOrDefaultAsync(It.IsAny<Specifications.Patient.PatientIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientId); // Same ID
            var validator = new UpdatePatientValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task UpdateValidator_EmailIsNotUsedByAnyPatient_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new UpdatePatientCommand(patientId, patientData);
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            repoMock.Setup(p => p.SingleOrDefaultAsync(It.IsAny<Specifications.Patient.PatientIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty); // Not used
            var validator = new UpdatePatientValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
