using Abstractions;
using Commands.Specialization;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Specialization;

namespace Validation.Tests.Specialization;

[TestFixture]
public class DeleteSpecializationValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Specialization>> _specRepoMock;
    private DeleteSpecializationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _specRepoMock = new Mock<IRepository<Domain.Entities.Specialization>>();
        _unitOfWorkMock.Setup(u => u.Specializations).Returns(_specRepoMock.Object);
        _validator = new DeleteSpecializationValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_IdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteSpecializationCommand(Guid.Empty);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
    }

    [Test]
    public async Task Validate_SpecializationDoesNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteSpecializationCommand(Guid.NewGuid());

        _specRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Specialization with this Id does not exist.");
    }

    [Test]
    public async Task Validate_SpecializationExists_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteSpecializationCommand(Guid.NewGuid());

        _specRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}