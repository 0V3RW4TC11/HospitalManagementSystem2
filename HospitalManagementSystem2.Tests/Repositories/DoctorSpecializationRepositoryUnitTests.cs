using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Repositories;

public class DoctorSpecializationRepositoryUnitTests : IDisposable, IAsyncDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DoctorSpecializationRepository _sut;

    public DoctorSpecializationRepositoryUnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new DoctorSpecializationRepository(_context);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void DoctorSpecializations_ReturnsIQueryable()
    {
        // Act
        var queryable = _sut.DoctorSpecializations;

        // Assert
        Assert.IsAssignableFrom<IQueryable<DoctorSpecialization>>(queryable);
    }

    [Fact]
    public async Task AddRangeAsync_ValidRange_AddsToDatabase()
    {
        // Arrange
        var range = new List<DoctorSpecialization>();
        var doctorId = Guid.NewGuid();
        for (var i = 0; i < 10; i++)
            range.Add(new DoctorSpecialization { DoctorId = doctorId, SpecializationId = Guid.NewGuid() });

        // Act
        await _sut.AddRangeAsync(range);
        _context.SaveChanges();

        // Assert
        Assert.Equal(_context.DoctorSpecializations.Count(), range.Count);
        foreach (var ds in range) Assert.Contains(ds, _context.DoctorSpecializations);
    }

    [Fact]
    public async Task AddRangeAsync_EmptyRange_NothingAddedToDatabase()
    {
        // Arrange
        var range = new List<DoctorSpecialization>();

        // Act
        await _sut.AddRangeAsync(range);

        // Assert
        Assert.Empty(_context.DoctorSpecializations);
    }

    [Fact]
    public async Task AddRangeAsync_NullInRange_Throws()
    {
        // Arrange
        var range = new List<DoctorSpecialization>
        {
            new() { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() },
            null!,
            new() { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() }
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.AddRangeAsync(range));
    }

    [Fact]
    public async Task AddRangeAsync_Null_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.AddRangeAsync(null!));
    }

    [Fact]
    public async Task RemoveRangeAsync_ExistingRange_RemovesFromDatabase()
    {
        // Arrange
        var range = new List<DoctorSpecialization>();
        for (var i = 0; i < 10; i++)
            range.Add(new DoctorSpecialization { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() });

        _context.DoctorSpecializations.AddRange(range);
        _context.SaveChanges();

        // Act
        await _sut.RemoveRangeAsync(range);
        _context.SaveChanges();

        // Assert
        Assert.Empty(_context.DoctorSpecializations);
    }

    [Fact]
    public async Task RemoveRangeAsync_NonExistingRange_Throws()
    {
        // Arrange
        var range = new List<DoctorSpecialization>();
        for (var i = 0; i < 10; i++)
            range.Add(new DoctorSpecialization { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() });

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveRangeAsync(range));
    }

    [Fact]
    public async Task RemoveRangeAsync_EmptyRange_NothingRemovedFromDatabase()
    {
        // Arrange
        var range = new List<DoctorSpecialization>();
        for (var i = 0; i < 10; i++)
            range.Add(new DoctorSpecialization { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() });

        _context.DoctorSpecializations.AddRange(range);
        _context.SaveChanges();

        // Act
        await _sut.RemoveRangeAsync(new List<DoctorSpecialization>());
        _context.SaveChanges();

        // Assert
        Assert.Equal(range.Count, _context.DoctorSpecializations.Count());
    }

    [Fact]
    public async Task RemoveRangeAsync_NullInRange_Throws()
    {
        // Arrange
        var completeRange = new List<DoctorSpecialization>
        {
            new() { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() },
            new() { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() },
            new() { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() }
        };
        var nullInRange = new List<DoctorSpecialization>
        {
            new() { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() },
            null!,
            new() { DoctorId = Guid.NewGuid(), SpecializationId = Guid.NewGuid() }
        };

        _context.DoctorSpecializations.AddRange(completeRange);
        _context.SaveChanges();

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveRangeAsync(nullInRange));
        Assert.Equal(completeRange.Count, _context.DoctorSpecializations.Count());
    }

    [Fact]
    public async Task RemoveRangeAsync_Null_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveRangeAsync(null));
    }
}