using Abstractions;
using Commands.Admin;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Admin;

namespace Validation.Tests.Admin;

[TestFixture]
public class UpdateAdminValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Admin>> _adminRepoMock;
    private UpdateAdminValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _adminRepoMock = new Mock<IRepository<Domain.Entities.Admin>>();
        _unitOfWorkMock.Setup(u => u.Admins).Returns(_adminRepoMock.Object);
        _validator = new UpdateAdminValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_IdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
        var command = new UpdateAdminCommand(Guid.Empty, adminData);

        _adminRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Admin>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Admins.SingleOrDefaultAsync(It.IsAny<Specifications.Admin.AdminIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty); // Not used

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
    }

    [Test]
    public async Task Validate_AdminDoesNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
        var command = new UpdateAdminCommand(Guid.NewGuid(), adminData);

        _adminRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Admin>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Admins.SingleOrDefaultAsync(It.IsAny<Specifications.Admin.AdminIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty); // Not used

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Admin with this Id does not exist.");
    }

    [Test]
    public async Task Validate_EmailIsUsedByAnotherAdmin_ShouldHaveValidationError()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
        var command = new UpdateAdminCommand(Guid.NewGuid(), adminData);

        _adminRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Admin>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Admins.SingleOrDefaultAsync(It.IsAny<Specifications.Admin.AdminIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid()); // Different id

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Data.Email).WithErrorMessage("This email is already used by another Admin");
    }

    [Test]
    public async Task Validate_EmailIsUsedBySameAdmin_ShouldNotHaveValidationError()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
        var command = new UpdateAdminCommand(adminId, adminData);

        _adminRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Admin>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Admins.SingleOrDefaultAsync(It.IsAny<Specifications.Admin.AdminIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminId); // Same id

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Data.Email);
    }

    [Test]
    public async Task Validate_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var adminData = new AdminData(null, "John", null, null, null, "1234567890", "test@example.com", null);
        var command = new UpdateAdminCommand(Guid.NewGuid(), adminData);

        _adminRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Admin>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Admins.SingleOrDefaultAsync(It.IsAny<Specifications.Admin.AdminIdByEmailSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty); // Not used

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}