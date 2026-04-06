using Abstractions;
using Commands.Admin;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Admin;

namespace Validation.Tests.Admin;

[TestFixture]
public class DeleteAdminValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Admin>> _adminRepoMock;
    private DeleteAdminValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _adminRepoMock = new Mock<IRepository<Domain.Entities.Admin>>();
        _unitOfWorkMock.Setup(u => u.Admins).Returns(_adminRepoMock.Object);
        _validator = new DeleteAdminValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_IdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteAdminCommand(Guid.Empty);

        _adminRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Admin>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
    }

    [Test]
    public async Task Validate_AdminDoesNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteAdminCommand(Guid.NewGuid());

        _adminRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Admin>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Admin with this Id does not exist.");
    }

    [Test]
    public async Task Validate_AdminExists_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteAdminCommand(Guid.NewGuid());

        _adminRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Admin>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}