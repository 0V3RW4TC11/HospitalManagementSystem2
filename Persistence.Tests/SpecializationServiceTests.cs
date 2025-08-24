using DataTransfer.Specialization;
using Domain.Exceptions;
using Mapster;
using TestData;

namespace Persistence.Tests;

internal sealed class SpecializationServiceTests : PersistenceTestBase
{
    [Test]
    public async Task CreateAsync_SpecializationWithValidData_CreatesSpecialization()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();

        // Act
        await GetServiceManager().SpecializationService.CreateAsync(spec1Dto);

        // Assert
        Assert.That(context.Specializations.Count, Is.EqualTo(1));;
        Assert.That(context.Specializations.Any(s => s.Name == spec1Dto.Name));
    }

    [Test]
    public void CreateAsync_SpecializationWithInvalidData_Throws()
    {
        // Arrange
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        spec1Dto.Name = string.Empty;
        
        // Act & Assert
        Assert.CatchAsync<Exception>(() => 
            GetServiceManager().SpecializationService.CreateAsync(spec1Dto));
    }
    
    [Test]
    public async Task CreateAsync_SpecializationWithDuplicateData_Throws()
    {
        // Arrange
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        await SpecializationTestData.SeedSpecializationsAsync(GetDbContext(), spec1Dto);
        
        // Act & Assert
        Assert.ThrowsAsync<SpecializationDuplicationException>(() =>
            GetServiceManager().SpecializationService.CreateAsync(spec1Dto));
    }
    
    [Test]
    public async Task GetAllAsync_ExistingSpecializations_ReturnsSpecializations()
    {
        // Arrange
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var spec2Dto = SpecializationTestData.CreateSpec2Dto();
        await SpecializationTestData.SeedSpecializationsAsync(GetDbContext(), spec1Dto, spec2Dto);
        
        // Act
        var result = 
            (await GetServiceManager().SpecializationService.GetAllAsync()).ToArray();
        
        // Assert
        Assert.That(result, Has.Length.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result.Any(s => s.Name == spec1Dto.Name));
            Assert.That(result.Any(s => s.Name == spec2Dto.Name));
        });
    }
    
    [Test]
    public async Task GetAllAsync_NoSpecializations_ReturnsNoSpecializations()
    {
        // Act
        var result = await GetServiceManager().SpecializationService.GetAllAsync();
        
        // Assert
        Assert.That(result, Is.Empty);
    }
    
    [Test]
    public async Task UpdateAsync_ExistingSpecializationWithValidData_UpdatesSpecialization()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var seeded = 
            await SpecializationTestData.SeedSpecializationsAsync(context, spec1Dto);
        var specToUpdate = seeded.First();
        specToUpdate.Name = "UpdatedName";
        
        // Act
        await GetServiceManager().SpecializationService.UpdateAsync(specToUpdate);
        
        // Assert
        Assert.That(context.Specializations.Any(s => 
            s.Id == specToUpdate.Id && 
            s.Name == specToUpdate.Name));
    }
    
    [Test]
    public async Task UpdateAsync_ExistingSpecializationWithInvalidData_Throws()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var seeded = 
            await SpecializationTestData.SeedSpecializationsAsync(context, spec1Dto);
        var specToUpdate = seeded.First();
        specToUpdate.Name = "";
        
        // Act & Assert
        Assert.CatchAsync<Exception>(() =>
            GetServiceManager().SpecializationService.UpdateAsync(specToUpdate));
    }
    
    [Test]
    public void UpdateAsync_NonExistingSpecialization_Throws()
    {
        // Arrange
        var spec1Dto = SpecializationTestData.CreateSpec1Dto().Adapt<SpecializationDto>();
        spec1Dto.Id = Guid.NewGuid();
        
        // Act & Assert
        Assert.ThrowsAsync<SpecializationNotFoundException>(() =>
            GetServiceManager().SpecializationService.UpdateAsync(spec1Dto));
    }
    
    [Test]
    public async Task DeleteAsync_ExistingSpecialization_DeletesSpecialization()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var seeded = 
            await SpecializationTestData.SeedSpecializationsAsync(context, spec1Dto);
        
        // Act
        await GetServiceManager().SpecializationService.DeleteAsync(seeded.First().Id);
        
        // Assert
        Assert.That(context.Specializations, Is.Empty);
    }
    
    [Test]
    public void DeleteAsync_NonExistingSpecialization_Throws()
    {
        // Act & Assert
        Assert.ThrowsAsync<SpecializationNotFoundException>(() =>
            GetServiceManager().SpecializationService.DeleteAsync(Guid.NewGuid()));
    }
}