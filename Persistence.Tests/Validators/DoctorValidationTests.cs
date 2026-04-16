using Abstractions;
using Commands.Doctor;
using FluentValidation.TestHelper;
using Moq;
using Validation.Doctor;

namespace Tests.Validators
{
    // TODO: Tests for Doctor ID

    [TestFixture]
    internal class DoctorValidationTests
    {
        private readonly DoctorValidator _doctorValidator = new();

        private static void CreateUowDoctorRepoMocks(
            out Mock<IUnitOfWork> uowMock,
            out Mock<IRepository<Domain.Entities.Doctor>> repoMock)
        {
            uowMock = new();
            repoMock = new();
            uowMock.Setup(u => u.Doctors).Returns(repoMock.Object);
        }


        [Test]
        public async Task DoctorValidator_FirstNameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var doctorData = new DoctorData("", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), [Guid.NewGuid()]);

            // Act
            var result = await _doctorValidator.TestValidateAsync(doctorData);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("First name is required");
        }

        [Test]
        public async Task DoctorValidator_PhoneIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "", "test@example.com", new DateOnly(1980, 1, 1), [Guid.NewGuid()]);

            // Act
            var result = await _doctorValidator.TestValidateAsync(doctorData);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Phone).WithErrorMessage("Phone number is required");
        }

        [Test]
        public async Task DoctorValidator_EmailIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "", new DateOnly(1980, 1, 1), [Guid.NewGuid()]);

            // Act
            var result = await _doctorValidator.TestValidateAsync(doctorData);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is required");
        }

        [Test]
        public async Task DoctorValidator_SpecListIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), []);

            // Act
            var result = await _doctorValidator.TestValidateAsync(doctorData);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SpecializationIds).WithErrorMessage("A Specialization is required.");
        }

        [Test]
        public async Task DoctorValidator_ValidDoctorData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), [Guid.NewGuid()]);

            // Act
            var result = await _doctorValidator.TestValidateAsync(doctorData);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task CreateValidator_PasswordIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
            var command = new CreateDoctorCommand(doctorData, "");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var validator = new CreateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password is required.");
        }

        [Test]
        public async Task CreateValidator_EmailIsNotUnique_ShouldHaveValidationError()
        {
            // Arrange
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
            var command = new CreateDoctorCommand(doctorData, "password");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var validator = new CreateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Doctor.");
        }

        [Test]
        public async Task CreateValidator_EmailIsUnique_ShouldNotHaveValidationError()
        {
            // Arrange
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
            var command = new CreateDoctorCommand(doctorData, "password");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var validator = new CreateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
        }

        [Test]
        public async Task CreateValidator_InvalidDoctorData_ShouldHaveValidationErrors()
        {
            // Arrange
            var doctorData = new DoctorData("", "Doe", "Male", "Address", "", "", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
            var command = new CreateDoctorCommand(doctorData, "password");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var validator = new CreateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.FirstName);
            result.ShouldHaveValidationErrorFor(x => x.Data.Phone);
            result.ShouldHaveValidationErrorFor(x => x.Data.Email);
        }

        [Test]
        public async Task CreateValidator_SpecializationIdsNotExist_ShouldHaveValidationError()
        {
            // Arrange
            var specId = Guid.NewGuid();
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { specId });
            var command = new CreateDoctorCommand(doctorData, "password");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Specifications.Doctor.DoctorByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0); // None exist
            var validator = new CreateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.SpecializationIds).WithErrorMessage("One or more Specializations do not exist.");
        }

        [Test]
        public async Task UpdateValidator_EmailIsUsedByAnotherDoctor_ShouldHaveValidationError()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var anotherDoctorId = Guid.NewGuid();
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
            var command = new UpdateDoctorCommand(doctorId, doctorData);
            CreateUowDoctorRepoMocks(out var uowMock, out var repoMock);
            uowMock.Setup(u => u.Doctors.SingleOrDefaultAsync(It.IsAny<Specifications.Doctor.DoctorIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(anotherDoctorId); // Email used by another doctor
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var validator = new UpdateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Doctor.");
        }

        [Test]
        public async Task UpdateValidator_InvalidDoctorData_ShouldHaveValidationErrors()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var doctorData = new DoctorData("", "Doe", "Male", "Address", "", "", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
            var command = new UpdateDoctorCommand(doctorId, doctorData);
            CreateUowDoctorRepoMocks(out var uowMock, out var repoMock);
            uowMock.Setup(u => u.Doctors.SingleOrDefaultAsync(It.IsAny<Specifications.Doctor.DoctorIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty);
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var validator = new UpdateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.FirstName);
            result.ShouldHaveValidationErrorFor(x => x.Data.Phone);
            result.ShouldHaveValidationErrorFor(x => x.Data.Email);
        }

        [Test]
        public async Task UpdateValidator_EmailIsUnique_ShouldNotHaveValidationError()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
            var command = new UpdateDoctorCommand(doctorId, doctorData);
            CreateUowDoctorRepoMocks(out var uowMock, out var repoMock);
            uowMock.Setup(u => u.Doctors.SingleOrDefaultAsync(It.IsAny<Specifications.Doctor.DoctorIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty); // Email not used
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var validator = new UpdateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
        }

        [Test]
        public async Task UpdateValidator_EmailIsSameAsCurrentDoctor_ShouldNotHaveValidationError()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var doctorData = new DoctorData("John", "Doe", "Male", "Address", "1234567890", "test@example.com", new DateOnly(1980, 1, 1), new List<Guid> { Guid.NewGuid() });
            var command = new UpdateDoctorCommand(doctorId, doctorData);
            CreateUowDoctorRepoMocks(out var uowMock, out var repoMock);
            uowMock.Setup(u => u.Doctors.SingleOrDefaultAsync(It.IsAny<Specifications.Doctor.DoctorIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorId); // Email used by same doctor
            uowMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var validator = new UpdateDoctorValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
        }
    }
}
