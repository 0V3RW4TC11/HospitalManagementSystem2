using DataTransfer.Attendance;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;
using TestData;

namespace Persistence.Tests;

internal class AttendanceServiceTests : PersistenceTestBase
{
    private async Task<Guid> SeedPatient()
    {
        var patient = PatientTestData.CreateDto().Adapt<Patient>();
        var context = GetDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        return patient.Id;
    }
    
    private async Task<Guid> SeedDoctor()
    {
        var doctor = DoctorTestData.CreateDto(new HashSet<Guid>()).Adapt<Doctor>();
        var context = GetDbContext();
        context.Doctors.Add(doctor);
        await context.SaveChangesAsync();
        return doctor.Id;
    }

    [Test]
    public async Task CreateAsync_ValidData_CreatesAttendance()
    {
        // Arrange
        var context = GetDbContext();
        var patientId = await SeedPatient();
        var doctorId = await SeedDoctor();
        var attendanceCreateDto = AttendanceTestData.CreateDto(patientId, doctorId);
        
        // Act
        await GetServiceManager().AttendanceService.CreateAsync(attendanceCreateDto);
        
        // Assert
        Assert.That(context.Attendances.Count, Is.EqualTo(1));
        Assert.That(context.Attendances.Any(a => 
            a.PatientId == patientId &&
            a.DoctorId == doctorId &&
            a.DateTime == attendanceCreateDto.DateTime &&
            a.Diagnosis == attendanceCreateDto.Diagnosis &&
            a.Remarks == attendanceCreateDto.Remarks &&
            a.Therapy == attendanceCreateDto.Therapy));
    }
    
    [Test]
    public void CreateAsync_InvalidDetails_Throws()
    {
        // Arrange
        var attendanceCreateDto = AttendanceTestData.CreateDto(Guid.NewGuid(), Guid.NewGuid());
        attendanceCreateDto.DateTime = DateTime.MinValue;
        attendanceCreateDto.Diagnosis = string.Empty;
        attendanceCreateDto.Remarks = string.Empty;
        attendanceCreateDto.Therapy = string.Empty;
        
        // Act & Assert
        Assert.CatchAsync<Exception>(() => 
            GetServiceManager().AttendanceService.CreateAsync(attendanceCreateDto));
    }
    
    [Test]
    public async Task CreateAsync_DuplicateDetails_Throws()
    {
        // Arrange
        var patientId = await SeedPatient();
        var doctorId = await SeedDoctor();
        var seededAttendanceDto = 
            await AttendanceTestData.SeedAttendance(GetDbContext(), patientId, doctorId);
        
        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => 
            GetServiceManager().AttendanceService.CreateAsync(seededAttendanceDto));
    }
    
    [Test]
    public async Task GetAllByPatientIdAsync_ExistingPatient_ReturnsAttendances()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var seeded = await AttendanceTestData.SeedAttendance(GetDbContext(), patientId, doctorId);
        
        // Act
        var result = 
            await GetServiceManager().AttendanceService.GetAllByPatientIdAsync(patientId);
        
        // Assert
        var attendance = result.Single();
        Assert.Multiple(() =>
        {
            Assert.That(attendance.Id, Is.EqualTo(seeded.Id));
            Assert.That(attendance.UserId, Is.EqualTo(doctorId));
            Assert.That(attendance.DateTime, Is.EqualTo(seeded.DateTime));
        });
    }
    
    [Test]
    public async Task GetAllByDoctorIdAsync_ExistingDoctor_ReturnsAttendances()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var seeded = await AttendanceTestData.SeedAttendance(GetDbContext(), patientId, doctorId);
        
        // Act
        var result = 
            await GetServiceManager().AttendanceService.GetAllByDoctorIdAsync(doctorId);
        
        // Assert
        var attendance = result.Single();
        Assert.Multiple(() =>
        {
            Assert.That(attendance.Id, Is.EqualTo(seeded.Id));
            Assert.That(attendance.UserId, Is.EqualTo(patientId));
            Assert.That(attendance.DateTime, Is.EqualTo(seeded.DateTime));
        });
    }
    
    [Test]
    public void GetAllByPatientIdAsync_NonExistingPatient_Throws()
    {
        // Act & Assert
        Assert.ThrowsAsync<PatientNotFoundException>(() =>
            GetServiceManager().AttendanceService.GetAllByPatientIdAsync(Guid.NewGuid()));
    }
    
    [Test]
    public void GetAllByDoctorIdAsync_NonExistingDoctor_ReturnsEmptyAttendances()
    {
        // Act & Assert
        Assert.ThrowsAsync<DoctorNotFoundException>(() =>
            GetServiceManager().AttendanceService.GetAllByDoctorIdAsync(Guid.NewGuid()));
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingAttendance_ReturnsAttendance()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var attendanceDto = await AttendanceTestData.SeedAttendance(GetDbContext(), patientId, doctorId);
        
        // Act
        var result = await GetServiceManager().AttendanceService.GetByIdAsync(attendanceDto.Id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(attendanceDto.Id));
            Assert.That(result.PatientId, Is.EqualTo(patientId));
            Assert.That(result.DoctorId, Is.EqualTo(doctorId));
            Assert.That(result.DateTime, Is.EqualTo(attendanceDto.DateTime));
            Assert.That(result.Diagnosis, Is.EqualTo(attendanceDto.Diagnosis));
            Assert.That(result.Remarks, Is.EqualTo(attendanceDto.Remarks));
            Assert.That(result.Therapy, Is.EqualTo(attendanceDto.Therapy));
        });
    }
    
    [Test]
    public void GetByIdAsync_NonExistingAttendance_Throws()
    {
        // Act & Assert
        Assert.CatchAsync<AttendanceNotFoundException>(() =>
            GetServiceManager().AttendanceService.GetByIdAsync(Guid.NewGuid()));
    }
    
    [Test]
    public async Task UpdateAsync_ExistingAttendanceWithValidData_UpdatesAttendance()
    {
        // Arrange
        var context = GetDbContext();
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var attendanceDto = await AttendanceTestData.SeedAttendance(context, patientId, doctorId);
        attendanceDto.PatientId = await SeedPatient();
        attendanceDto.DoctorId = await SeedDoctor();
        attendanceDto.DateTime = DateTime.Now + TimeSpan.FromDays(1);
        attendanceDto.Diagnosis = "UpdatedDiagnosis";
        attendanceDto.Remarks = "UpdatedRemarks";
        attendanceDto.Therapy = "UpdatedTherapy";
        
        // Act
        await GetServiceManager().AttendanceService.UpdateAsync(attendanceDto);
        
        // Assert
        var result = context.Attendances.Single(a => a.Id == attendanceDto.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result.PatientId, Is.EqualTo(attendanceDto.PatientId));
            Assert.That(result.DoctorId, Is.EqualTo(attendanceDto.DoctorId));
            Assert.That(result.DateTime, Is.EqualTo(attendanceDto.DateTime));
            Assert.That(result.Diagnosis, Is.EqualTo(attendanceDto.Diagnosis));
            Assert.That(result.Remarks, Is.EqualTo(attendanceDto.Remarks));
            Assert.That(result.Therapy, Is.EqualTo(attendanceDto.Therapy));
        });
    }
    
    [Test]
    public async Task UpdateAsync_ExistingAttendanceWithInvalidData_Throws()
    {
        // Arrange
        var context = GetDbContext();
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var attendanceDto = await AttendanceTestData.SeedAttendance(context, patientId, doctorId);
        attendanceDto.PatientId = Guid.NewGuid();
        attendanceDto.DoctorId = Guid.NewGuid();
        attendanceDto.DateTime = DateTime.MinValue;
        attendanceDto.Diagnosis = string.Empty;
        attendanceDto.Remarks = string.Empty;
        attendanceDto.Therapy = string.Empty;
        
        // Act & Assert
        Assert.CatchAsync<Exception>(() => 
            GetServiceManager().AttendanceService.UpdateAsync(attendanceDto));
    }
    
    [Test]
    public void UpdateAsync_NonExistingAttendance_Throws()
    {
        // Arrange
        var attendanceDto = AttendanceTestData.CreateDto(Guid.NewGuid(), Guid.NewGuid())
            .Adapt<AttendanceDto>();
        attendanceDto.Id = Guid.NewGuid();
        
        // Act & Assert
        Assert.CatchAsync<AttendanceNotFoundException>(() => 
            GetServiceManager().AttendanceService.UpdateAsync(attendanceDto));
    }
    
    [Test]
    public async Task DeleteAsync_ExistingAttendance_DeletesAttendance()
    {
        // Arrange
        var context = GetDbContext();
        var attendanceDto = 
            await AttendanceTestData.SeedAttendance(context, Guid.NewGuid(), Guid.NewGuid());
        
        // Act
        await GetServiceManager().AttendanceService.DeleteAsync(attendanceDto.Id);
        
        // Assert
        Assert.That(context.Attendances, Is.Empty);
    }
    
    [Test]
    public void DeleteAsync_NonExistingAttendance_Throws()
    {
        // Act & Assert
        Assert.CatchAsync<AttendanceNotFoundException>(() => 
            GetServiceManager().AttendanceService.DeleteAsync(Guid.NewGuid()));
    }
}