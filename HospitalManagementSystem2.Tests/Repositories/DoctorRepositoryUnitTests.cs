using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace HospitalManagementSystem2.Tests.Repositories;

public class DoctorRepositoryUnitTests : IDisposable, IAsyncDisposable
{
    private const string TestFirstName = "ExampleFirstName";
    private const string TestLastName = "ExampleLastName";
    private const string TestGender = "ExampleGender";
    private const string TestAddress = "ExampleAddress";
    private const string TestPhone = "ExamplePhone";
    private const string TestEmail = "ExampleEmail";
    private static readonly DateOnly TestDateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch);
    private readonly ApplicationDbContext _context;
    private readonly Mock<IDoctorSpecializationRepository> _mockDocSpecRepo;
    private readonly Mock<ISpecializationRepository> _mockSpecRepo;
    private readonly DoctorRepository _sut;

    public DoctorRepositoryUnitTests()
    {
        _context = InMemoryDbHelper.CreateInMemDb();
        _mockDocSpecRepo = new Mock<IDoctorSpecializationRepository>();
        _mockSpecRepo = new Mock<ISpecializationRepository>();
        _sut = new DoctorRepository(_context, _mockDocSpecRepo.Object, _mockSpecRepo.Object);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private void SetupDSMockQueryable()
    {
        _mockDocSpecRepo.Setup(r => r.DoctorSpecializations)
            .Returns(_context.DoctorSpecializations.AsNoTracking());
    }

    private void SetupDSMockAddRange()
    {
        _mockDocSpecRepo.Setup(r
                => r.AddRangeAsync(It.IsAny<IEnumerable<DoctorSpecialization>>()))
            .Callback((IEnumerable<DoctorSpecialization> range) => { _context.DoctorSpecializations.AddRange(range); })
            .Returns(Task.CompletedTask);
    }

    private void SetupDSMockRemoveRange()
    {
        _mockDocSpecRepo.Setup(r
                => r.RemoveRangeAsync(It.IsAny<IEnumerable<DoctorSpecialization>>()))
            .Callback((IEnumerable<DoctorSpecialization> range) =>
            {
                var toRemove = new List<DoctorSpecialization>();
                foreach (var ds in range) toRemove.Add(_context.DoctorSpecializations.First(x => x == ds));

                _context.DoctorSpecializations.RemoveRange(toRemove);
            })
            .Returns(Task.CompletedTask);
    }

    private void SetupSpecMockQueryable()
    {
        _mockSpecRepo.Setup(r => r.Specializations)
            .Returns(_context.Specializations.AsNoTracking());
    }

    [Fact]
    public async Task AddAsync_NewDoctorWithSpecs_AddsToDatabase()
    {
        // Arrange
        SetupDSMockAddRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var specs = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec1" },
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec2" }
        };
        doctor.Specializations = specs;

        // Act
        await _sut.AddAsync(doctor);
        _context.SaveChanges();

        // Assert
        Assert.Contains(doctor, _context.Doctors);
        foreach (var spec in specs)
            Assert.Contains(_context.DoctorSpecializations, ds => ds.SpecializationId == spec.Id);
    }

    [Fact]
    public async Task AddAsync_NullDoctor_Throws()
    {
        // Arrange
        SetupDSMockAddRange();

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => _sut.AddAsync(null!));
    }

    [Fact]
    public async Task AddAsync_NewDoctorWithNullSpecs_Throws()
    {
        // Arrange
        SetupDSMockAddRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => _sut.AddAsync(doctor));
    }

    [Fact]
    public async Task AddAsync_NewDoctorWithEmptySpecs_DoctorAddedWithoutSpecs()
    {
        // Arrange
        SetupDSMockAddRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth,
            Specializations = new List<Specialization>()
        };

        // Act
        await _sut.AddAsync(doctor);
        _context.SaveChanges();

        // Assert
        Assert.Contains(doctor, _context.Doctors);
        Assert.Empty(_context.DoctorSpecializations);
    }

    [Fact]
    public async Task AddAsync_ExistingDoctor_DuplicateAddedToDatabase()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupDSMockAddRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth,
            Specializations = new List<Specialization>()
        };
        var specs = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec1" },
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec2" }
        };
        doctor.Specializations = specs;
        _context.Doctors.Add(doctor);
        _context.SaveChanges();
        foreach (var spec in specs)
            _context.DoctorSpecializations.Add(new DoctorSpecialization
                { DoctorId = doctor.Id, SpecializationId = spec.Id });
        _context.SaveChanges();
        doctor.Id = Guid.Empty;
        
        // Act
        await _sut.AddAsync(doctor);
        Assert.Single(_context.Doctors);
        Assert.Equal(specs.Count, _context.DoctorSpecializations.Count());
    }

    [Fact]
    public async Task UpdateAsync_ExistingDoctor_UpdatesDoctor()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupDSMockAddRange();
        SetupDSMockRemoveRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var specs = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec1" },
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec2" }
        };
        doctor.Specializations = specs;

        _context.Doctors.Add(doctor);
        _context.SaveChanges();

        foreach (var spec in specs)
            _context.DoctorSpecializations.Add(new DoctorSpecialization
                { DoctorId = doctor.Id, SpecializationId = spec.Id });
        _context.SaveChanges();

        doctor.FirstName = "ChangedFirstName";
        doctor.LastName = "ChangedLastName";
        var docSpecs = doctor.Specializations.ToList();
        docSpecs.RemoveAt(1);
        doctor.Specializations = docSpecs;

        // Act
        await _sut.UpdateAsync(doctor);
        _context.SaveChanges();

        // Assert
        var result = _context.Doctors.First(d => d.Id == doctor.Id);
        Assert.Equal(doctor.FirstName, result.FirstName);
        Assert.Equal(doctor.LastName, result.LastName);
        Assert.Equal(doctor.Specializations.Count(), _context.DoctorSpecializations.Count());
    }

    [Fact]
    public async Task UpdateAsync_ExistingDoctorWithEmptySpecs_RemovesSpecs()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupDSMockAddRange();
        SetupDSMockRemoveRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var specs = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec1" },
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec2" }
        };
        doctor.Specializations = specs;

        _context.Doctors.Add(doctor);
        _context.SaveChanges();

        foreach (var spec in specs)
            _context.DoctorSpecializations.Add(new DoctorSpecialization
                { DoctorId = doctor.Id, SpecializationId = spec.Id });
        _context.SaveChanges();

        doctor.Specializations = new List<Specialization>();

        // Act
        await _sut.UpdateAsync(doctor);
        _context.SaveChanges();

        // Assert
        Assert.Equal(doctor.Specializations.Count(), _context.DoctorSpecializations.Count());
    }

    [Fact]
    public async Task UpdateAsync_NonExistingDoctor_Throws()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupDSMockAddRange();
        SetupDSMockRemoveRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var specs = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec1" },
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec2" }
        };
        doctor.Specializations = specs;

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.UpdateAsync(doctor));
    }

    [Fact]
    public async Task UpdateAsync_NullDoctor_Throws()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupDSMockAddRange();
        SetupDSMockRemoveRange();

        await Assert.ThrowsAnyAsync<Exception>(() => _sut.UpdateAsync(null));
    }

    [Fact]
    public async Task RemoveAsync_ExistingDoctor_RemovesDoctorAndDocSpecs()
    {
        // Arrange
        SetupDSMockRemoveRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var specs = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec1" },
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec2" }
        };
        doctor.Specializations = specs;

        _context.Doctors.Add(doctor);
        _context.SaveChanges();

        foreach (var spec in specs)
            _context.DoctorSpecializations.Add(new DoctorSpecialization
                { DoctorId = doctor.Id, SpecializationId = spec.Id });
        _context.SaveChanges();

        // Act
        await _sut.RemoveAsync(doctor);
        _context.SaveChanges();

        // Assert
        Assert.DoesNotContain(doctor, _context.Doctors);
        Assert.Empty(_context.DoctorSpecializations);
    }

    [Fact]
    public async Task RemoveAsync_NonExistingDoctor_Throws()
    {
        // Arrange
        SetupDSMockRemoveRange();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var specs = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec1" },
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec2" }
        };
        doctor.Specializations = specs;

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveAsync(doctor));
    }

    [Fact]
    public async Task RemoveAsync_NullDoctor_Throws()
    {
        // Arrange
        SetupDSMockRemoveRange();
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.UpdateAsync(null));
    }
    
    [Fact]
    public async Task FindByIdAsync_ExistingDoctor_ReturnsDoctor()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupSpecMockQueryable();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        var specs = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec1" },
            new() { Id = Guid.NewGuid(), Name = "ExampleSpec2" }
        };
        doctor.Specializations = specs;

        _context.Doctors.Add(doctor);
        _context.SaveChanges();
        foreach (var spec in specs)
            _context.DoctorSpecializations.Add(new DoctorSpecialization
                { DoctorId = doctor.Id, SpecializationId = spec.Id });
        _context.SaveChanges();
        
        // Act
        var result = await _sut.FindByIdAsync(doctor.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(doctor, result);
        Assert.False(result.Specializations.IsNullOrEmpty());
        foreach (var ds in doctor.Specializations)
        {
            Assert.Contains(result.Specializations, rs => rs.Id == ds.Id);
        }
    }

    [Fact]
    public async Task FindByIdAsync_NonExistingDoctor_ReturnsNull()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupSpecMockQueryable();

        // Act
        var result = await  _sut.FindByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task FindByIdAsync_DoctorWithNoId_ReturnsNull()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupSpecMockQueryable();

        var doctor = new Doctor
        {
            Id = Guid.Empty
        };

        // Act
        var result = await  _sut.FindByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task FindByIdAsync_ExistingDoctorWithNoSpecs_ReturnsDoctorWithEmptySpecs()
    {
        // Arrange
        SetupDSMockQueryable();
        SetupSpecMockQueryable();

        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };

        _context.Doctors.Add(doctor);
        _context.SaveChanges();
        
        // Act
        var result = await _sut.FindByIdAsync(doctor.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Specializations);
        Assert.Empty(result.Specializations);
    }
}