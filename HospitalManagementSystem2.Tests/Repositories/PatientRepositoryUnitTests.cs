using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Tests.Helpers;
using HospitalManagementSystem2.Tests.TestData;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Repositories;

public class PatientRepositoryUnitTests : IDisposable, IAsyncDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PatientRepository _sut;
        
    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    public PatientRepositoryUnitTests()
    {
        _context = InMemoryDbHelper.CreateInMemDb();
        _sut = new PatientRepository(_context);
    }
    
    [Fact]
    public async Task AddAsync_NewPatient_AddsToDatabase()
    {
        // Arrange
        var patient = PatientTestHelper.CreatePatient();
        
        // Act
        await _sut.AddAsync(patient);
        _context.SaveChanges();
        
        // Assert
        var result = _context.Patients.FirstOrDefault(p => p == patient);
        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
        Assert.Equal(patient.Title, result.Title);
        Assert.Equal(patient.FirstName, result.FirstName);
        Assert.Equal(patient.LastName, result.LastName);
        Assert.Equal(patient.Gender, result.Gender);
        Assert.Equal(patient.Address, result.Address);
        Assert.Equal(patient.Phone, result.Phone);
        Assert.Equal(patient.Email, result.Email);
        Assert.Equal(patient.DateOfBirth, result.DateOfBirth);
        Assert.Equal(patient.BloodType, result.BloodType);
    }
    
    [Fact]
    public async Task AddAsync_NewPatientMissingDetails_Throws()
    {
        // Arrange
        var patient = PatientTestHelper.CreatePatient();
        patient.FirstName = null!;
        patient.LastName = null;
        
        // Act
        await _sut.AddAsync(patient);
        
        // Assert
        var result = Assert.ThrowsAny<Exception>(() => _context.SaveChanges());
        Assert.Contains("Required properties '{'FirstName'}' are missing", result.Message);
    }
    
    [Fact]
    public async Task AddAsync_DuplicatePatient_AddsDuplicateToDatabase()
    {
        // Arrange
        PatientTestHelper.CreateAndSeedPatient(_context);
        var duplicate = PatientTestHelper.CreatePatient();
        
        // Act
        await _sut.AddAsync(duplicate);
        _context.SaveChanges();
        
        // Assert
        Assert.Equal(2, _context.Patients.Count());
        var patientAId = _context.Patients.First().Id;
        var patientBId = _context.Patients.Skip(1).First().Id;
        Assert.NotEqual(patientAId, patientBId);
    }

    [Fact] 
    public async Task AddAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.AddAsync(null!));
        Assert.Contains("Value cannot be null", result.Message);
    }
    
    [Fact] 
    public async Task UpdateAsync_ExistingPatient_UpdatesInDatabase()
    {
        // Arrange
        var patient = PatientTestHelper.CreateAndSeedPatient(_context);
        patient.FirstName = "UpdatedFirstName";
        patient.LastName = "UpdatedLastName";
        patient.BloodType = BloodType.OPositive;
        
        // Act
        await _sut.UpdateAsync(patient);
        _context.SaveChanges();
        
        // Assert
        var entry = _context.Patients.FirstOrDefault(p => p.Id == patient.Id);
        Assert.NotNull(entry);
        Assert.Equal(patient.FirstName, entry.FirstName);
        Assert.Equal(patient.LastName, entry.LastName);
        Assert.Equal(patient.BloodType, entry.BloodType);
    }
    
    [Fact] 
    public async Task UpdateAsync_NonExistingPatient_Throws()
    {
        // Arrange
        var patient = PatientTestHelper.CreatePatient();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.UpdateAsync(patient));
        Assert.Contains("Sequence contains no elements", result.Message);
    }
    
    [Fact] 
    public async Task UpdateAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.UpdateAsync(null));
        Assert.Contains("An exception was thrown while attempting to evaluate " +
                        "a LINQ query parameter expression", result.Message);
    }
    
    [Fact] 
    public async Task RemoveAsync_ExistingPatient_RemovesFromDatabase()
    {
        // Arrange
        var patient = PatientTestHelper.CreateAndSeedPatient(_context);
        
        // Act
        await _sut.RemoveAsync(patient);
        _context.SaveChanges();
        
        // Assert
        Assert.Empty(_context.Patients);
    }
    
    [Fact] 
    public async Task RemoveAsync_NonExistingPatient_Throws()
    {
        // Arrange
        var patient = PatientTestHelper.CreatePatient();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveAsync(patient));
        Assert.Contains("Sequence contains no elements", result.Message);
    }
    
    [Fact] 
    public async Task RemoveAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveAsync(null!));
        Assert.Contains("An exception was thrown while attempting to evaluate " +
                        "a LINQ query parameter expression", result.Message);
    }
}