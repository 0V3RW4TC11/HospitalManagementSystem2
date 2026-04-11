using Abstractions;
using Commands.Patient;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Patient;

namespace Validation.Tests.Patient
{
    [TestFixture]
    internal class UpdatePatientValidatorTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IRepository<Domain.Entities.Patient>> _patientsMock;
        private UpdatePatientValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new();
            _patientsMock = new();
            _unitOfWorkMock.Setup(u => u.Patients).Returns(_patientsMock.Object);
            _validator = new(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task Validate_EmailIsUsedByAnotherPatient_ShouldHaveValidationError()
        {
            // Arrange
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new UpdatePatientCommand(Guid.NewGuid(), patientData);

            _patientsMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _patientsMock.Setup(p => p.SingleOrDefaultAsync(It.IsAny<Specifications.Patient.PatientIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid()); // Different ID

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Patient");
        }

        [Test]
        public async Task Validate_ValidCommand_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new UpdatePatientCommand(patientId, patientData);

            _patientsMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _patientsMock.Setup(p => p.SingleOrDefaultAsync(It.IsAny<Specifications.Patient.PatientIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientId); // Same ID

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task Validate_EmailIsSameAsCurrent_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new UpdatePatientCommand(patientId, patientData);

            _patientsMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _patientsMock.Setup(p => p.SingleOrDefaultAsync(It.IsAny<Specifications.Patient.PatientIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientId); // Same ID

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task Validate_EmailIsNotUsedByAnyPatient_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patientData = new PatientData("", "FirstName", "", "Male", "", "", "email@example.com", Constants.BloodType.APositive, DateOnly.FromDateTime(DateTime.UnixEpoch));
            var command = new UpdatePatientCommand(patientId, patientData);

            _patientsMock.Setup(p => p.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Patient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _patientsMock.Setup(p => p.SingleOrDefaultAsync(It.IsAny<Specifications.Patient.PatientIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty); // Not used

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
