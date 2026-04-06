using Abstractions;
using Commands.Admin;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Admin;

namespace Validation.Tests.Admin;

[TestFixture]
public class CreateAdminValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CreateAdminValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new CreateAdminValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_PasswordIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
        var command = new CreateAdminCommand(adminData, "");

        _unitOfWorkMock.Setup(u => u.Admins.AnyAsync(It.IsAny<Specifications.Admin.AdminByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password is required.");
    }

    [Test]
    public async Task Validate_EmailIsNotUnique_ShouldHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
        var command = new CreateAdminCommand(adminData, "password");

        _unitOfWorkMock.Setup(u => u.Admins.AnyAsync(It.IsAny<Specifications.Admin.AdminByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Admin");
    }

    [Test]
    public async Task Validate_EmailIsUnique_ShouldNotHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
        var command = new CreateAdminCommand(adminData, "password");

        _unitOfWorkMock.Setup(u => u.Admins.AnyAsync(It.IsAny<Specifications.Admin.AdminByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
    }

    [Test]
    public async Task Validate_InvalidAdminData_ShouldHaveValidationErrors()
    {
        // Arrange
        var adminData = new AdminData(null, "", null, null, null, "", "", null);
        var command = new CreateAdminCommand(adminData, "");

        _unitOfWorkMock.Setup(u => u.Admins.AnyAsync(It.IsAny<Specifications.Admin.AdminByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.Data.Phone);
        result.ShouldHaveValidationErrorFor(x => x.Data.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}