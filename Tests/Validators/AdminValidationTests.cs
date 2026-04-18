using Abstractions;
using Commands.Admin;
using Entities;
using FluentValidation.TestHelper;
using Moq;
using Validation.Admin;

namespace Tests.Validators
{
    [TestFixture]
    internal class AdminValidationTests
    {
        private readonly AdminValidator _adminValidator = new();

        private static void CreateUowAndRepoMock(out Mock<IUnitOfWork> uowMock, out Mock<IRepository<Admin>> repoMock)
        {
            uowMock = new();
            repoMock = new();
            uowMock.Setup(u => u.Admins).Returns(repoMock.Object);
        }

        [Test]
        public void AdminValidator_FirstNameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var adminData = new AdminData(null, "", null, null, null, "1234567890", "test@example.com", null);

            // Act
            var result = _adminValidator.TestValidate(adminData);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("First name is required");
        }

        [Test]
        public void AdminValidator_PhoneIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var adminData = new AdminData(null, "John", null, null, null, "", "test@example.com", null);

            // Act
            var result = _adminValidator.TestValidate(adminData);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Phone).WithErrorMessage("Phone number is required");
        }

        [Test]
        public void AdminValidator_EmailIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var adminData = new AdminData(null, "John", null, null, null, "1234567890", "", null);

            // Act
            var result = _adminValidator.TestValidate(adminData);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is required");
        }

        [Test]
        public void AdminValidator_ValidAdminData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);

            // Act
            var result = _adminValidator.TestValidate(adminData);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task CreateValidator_PasswordIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
            var command = new CreateAdminCommand(adminData, "");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Admins.AnyAsync(It.IsAny<Specifications.Admin.AdminByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var validator = new CreateAdminValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password is required.");
        }

        [Test]
        public async Task CreateValidator_EmailIsNotUnique_ShouldHaveValidationError()
        {
            // Arrange
            var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
            var command = new CreateAdminCommand(adminData, "password");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Admins.AnyAsync(It.IsAny<Specifications.Admin.AdminByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new CreateAdminValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Admin");
        }

        [Test]
        public async Task CreateValidator_EmailIsUnique_ShouldNotHaveValidationError()
        {
            // Arrange
            var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
            var command = new CreateAdminCommand(adminData, "password");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Admins.AnyAsync(It.IsAny<Specifications.Admin.AdminByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var validator = new CreateAdminValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
        }

        [Test]
        public async Task CreateValidator_InvalidAdminData_ShouldHaveValidationErrors()
        {
            // Arrange
            var adminData = new AdminData(null, "", null, null, null, "", "", null);
            var command = new CreateAdminCommand(adminData, "");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Admins.AnyAsync(It.IsAny<Specifications.Admin.AdminByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var validator = new CreateAdminValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.FirstName);
            result.ShouldHaveValidationErrorFor(x => x.Data.Phone);
            result.ShouldHaveValidationErrorFor(x => x.Data.Email);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public async Task UpdateValidator_EmailIsUsedByAnotherAdmin_ShouldHaveValidationError()
        {
            // Arrange
            var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
            var command = new UpdateAdminCommand(Guid.NewGuid(), adminData);
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Admin>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            uowMock.Setup(u => u.Admins.SingleOrDefaultAsync(It.IsAny<Specifications.Admin.AdminIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid()); // Different id
            var validator = new UpdateAdminValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Admin");
        }

        [Test]
        public async Task UpdateValidator_EmailIsUsedBySameAdmin_ShouldNotHaveValidationError()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
            var command = new UpdateAdminCommand(adminId, adminData);
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Admin>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            uowMock.Setup(u => u.Admins.SingleOrDefaultAsync(It.IsAny<Specifications.Admin.AdminIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminId); // Same id
            var validator = new UpdateAdminValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
        }

        [Test]
        public async Task UpdateValidator_ValidData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
            var command = new UpdateAdminCommand(Guid.NewGuid(), adminData);
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Admin>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            uowMock.Setup(u => u.Admins.SingleOrDefaultAsync(It.IsAny<Specifications.Admin.AdminIdByEmailSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty); // Not used
            var validator = new UpdateAdminValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
