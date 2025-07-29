using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence.Tests;

internal sealed class PatientServiceTests : PersistenceTestBase
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        SeedPatientRole();
    }
    
    private void SeedPatientRole()
    {
        var roleManager = GetServiceProvider().GetRequiredService<RoleManager<IdentityRole>>();
        roleManager.CreateAsync(new IdentityRole(AuthRoles.Patient)).GetAwaiter().GetResult();
    }

    [Test]
    public async Task CreateAsync_PatientWithValidData_CreatesPatientAndAccount()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
    
    [Test]
    public async Task CreateAsync_PatientWithInvalidData_Throws()
    {
        // Arrange
        
        // Act & Assert
    }
    
    [Test]
    public async Task CreateAsync_PatientWithDuplicateData_Throws()
    {
        // Arrange
        
        // Act & Assert
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingPatient_ReturnsPatient()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
    
    [Test]
    public async Task GetByIdAsync_NonExistingPatient_Throws()
    {
        // Arrange
        
        // Act & Assert
    }
    
    [Test]
    public async Task UpdateAsync_ExistingPatientWithValidData_UpdatesPatient()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
    
    [Test]
    public async Task UpdateAsync_ExistingPatientWithInvalidData_Throws()
    {
        // Arrange
        
        // Act & Assert
    }
    
    [Test]
    public async Task UpdateAsync_NonExistingPatient_Throws()
    {
        // Arrange
        
        // Act & Assert
    }
    
    [Test]
    public async Task DeleteAsync_ExistingPatient_DeletesPatientAndAccount()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
    
    [Test]
    public async Task DeleteAsync_NonExistingPatient_Throws()
    {
        // Arrange
        
        // Act & Assert
    }
}