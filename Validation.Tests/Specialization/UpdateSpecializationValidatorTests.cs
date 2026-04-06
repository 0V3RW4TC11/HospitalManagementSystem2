using Abstractions;
using Commands.Specialization;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Specialization;

namespace Validation.Tests.Specialization;

[TestFixture]
public class UpdateSpecializationValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Specialization>> _specRepoMock;
    private UpdateSpecializationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _specRepoMock = new Mock<IRepository<Domain.Entities.Specialization>>();
        _unitOfWorkMock.Setup(u => u.Specializations).Returns(_specRepoMock.Object);
        _validator = new UpdateSpecializationValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_IdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateSpecializationCommand(Guid.Empty, "Name");

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
    }

    [Test]
    public async Task Validate_SpecializationDoesNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateSpecializationCommand(Guid.NewGuid(), "Name");

       _specRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Specialization with this Id does not exist.");
    }

    [Test]
    public async Task Validate_NameIsUsedByAnotherSpecialization_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateSpecializationCommand(Guid.NewGuid(), "Existing Name");

        _specRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Specializations.SingleOrDefaultAsync(It.IsAny<Specifications.Specialization.SpecializationIdByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid()); // Different id

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("This name is already used by another Specialization.");
    }

    [Test]
    public async Task Validate_NameIsUsedBySameSpecialization_ShouldNotHaveValidationError()
    {
        // Arrange
        var specializationId = Guid.NewGuid();
        var command = new UpdateSpecializationCommand(specializationId, "Existing Name");

        _specRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Specializations.SingleOrDefaultAsync(It.IsAny<Specifications.Specialization.SpecializationIdByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(specializationId); // Same id

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test]
    public async Task Validate_NameIsUnique_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateSpecializationCommand(Guid.NewGuid(), "Unique Name");

        _specRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Specializations.SingleOrDefaultAsync(It.IsAny<Specifications.Specialization.SpecializationIdByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty); // Not used

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x);
    }
}