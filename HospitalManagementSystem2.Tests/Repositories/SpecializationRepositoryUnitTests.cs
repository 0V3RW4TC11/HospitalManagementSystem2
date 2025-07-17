using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Tests.Helpers;

namespace HospitalManagementSystem2.Tests.Repositories;

public class SpecializationRepositoryUnitTests : IDisposable, IAsyncDisposable
{
    private const string TestName = "TestName";
    
    private readonly ApplicationDbContext _context;
    private readonly SpecializationRepository _sut;

    public SpecializationRepositoryUnitTests()
    {
        _context = InMemoryDbHelper.CreateInMemDb();
        _sut = new SpecializationRepository(_context);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_NewSpecialization_AddsToDatabase()
    {
        // Arrange
        var spec = new Specialization { Name = TestName };
        
        // Act
        await _sut.AddAsync(spec);
        _context.SaveChanges();
        
        // Assert
        Assert.Equal(1, _context.Specializations.Count());
        Assert.Contains(spec, _context.Specializations);
    }

    [Fact]
    public async Task AddAsync_ExistingSpecialization_AddsDuplicateToDatabase()
    {
        // Arrange
        var existingSpec = new Specialization { Name = TestName };
        _context.Specializations.Add(existingSpec);
        _context.SaveChanges();
        var newSpec = new Specialization { Name = TestName };
        
        // Act
        await _sut.AddAsync(newSpec);
        _context.SaveChanges();
        
        // Assert
        Assert.Equal(2, _context.Specializations.Count());
        Assert.Contains(existingSpec, _context.Specializations);
        Assert.Contains(newSpec, _context.Specializations);
    }

    [Fact]
    public async Task AddAsync_NullSpecialization_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.AddAsync(null!));
    }
    
    [Fact]
    public async Task AddAsync_NewSpecializationEmptyName_AddsSpecWithEmptyName()
    {
        // Arrange
        var spec = new Specialization { Name = string.Empty };
        
        // Act
        await _sut.AddAsync(spec);
        _context.SaveChanges();
        
        // Assert
        Assert.Equal(1, _context.Specializations.Count());
        Assert.Equal(string.Empty, _context.Specializations.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_ExistingSpecialization_UpdatesSpec()
    {
        // Arrange
        var existingSpec = new Specialization { Name = TestName };
        _context.Specializations.Add(existingSpec);
        _context.SaveChanges();
        existingSpec.Name = "UpdatedName";
        
        // Act
        await _sut.UpdateAsync(existingSpec);
        _context.SaveChanges();
        
        // Assert
        Assert.Equal(1, _context.Specializations.Count());
        Assert.Equal("UpdatedName", _context.Specializations.First().Name);
    }
    
    [Fact]
    public async Task UpdateAsync_NonExistingSpecialization_Throws()
    {
        // Arrange
        var spec = new Specialization { Name = TestName };
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.UpdateAsync(spec));
    }
    
    [Fact]
    public async Task UpdateAsync_ReplaceNameWithEmptyName_UpdatesSpecWithEmptyName()
    {
        // Arrange
        var existingSpec = new Specialization { Name = TestName };
        _context.Specializations.Add(existingSpec);
        _context.SaveChanges();
        existingSpec.Name = string.Empty;
        
        // Act
        await _sut.UpdateAsync(existingSpec);
        
        // Assert
        Assert.Equal(1, _context.Specializations.Count());
        Assert.Equal(string.Empty, _context.Specializations.First().Name);
    }
    
    [Fact]
    public async Task UpdateAsync_NullSpecialization_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.UpdateAsync(null!));
    }
    
    [Fact]
    public async Task RemoveAsync_ExistingSpecialization_RemovesSpec()
    {
        // Arrange
        var existingSpec = new Specialization { Name = TestName };
        _context.Specializations.Add(existingSpec);
        _context.SaveChanges();
        
        // Act
        await _sut.RemoveAsync(existingSpec);
        _context.SaveChanges();
        
        // Assert
        Assert.Empty(_context.Specializations);
    }
    
    [Fact]
    public async Task RemoveAsync_NonExistingSpecialization_Throws()
    {
        // Arrange
        var spec = new Specialization { Name = TestName };
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveAsync(spec));
    }
    
    [Fact]
    public async Task RemoveAsync_NullSpecialization_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveAsync(null!));
    }
}