using DataTransfer.Attendance;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;

namespace Services;

internal sealed class AttendanceService : IAttendanceService
{
    private readonly IRepositoryManager _repositoryManager;

    public AttendanceService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task CreateAsync(AttendanceCreateDto attendanceCreateDto)
    {
        ValidateAttendanceCreateDto(attendanceCreateDto);
        
        var attendance = attendanceCreateDto.Adapt<Attendance>();
        
        _repositoryManager.AttendanceRepository.Add(attendance);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<AttendanceDto>> GetAllByPatientIdAsync(Guid id)
    {
        var attendances = await _repositoryManager.AttendanceRepository.GetAllByPatientIdAsync(id);
        
        return attendances.Adapt<IEnumerable<AttendanceDto>>();
    }

    public async Task<IEnumerable<AttendanceDto>> GetAllByDoctorIdAsync(Guid id)
    {
        var attendances = await _repositoryManager.AttendanceRepository.GetAllByDoctorIdAsync(id);
        
        return attendances.Adapt<IEnumerable<AttendanceDto>>();
    }
    
    public async Task<AttendanceDto> GetByIdAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);
        
        return attendance.Adapt<AttendanceDto>();
    }

    public async Task UpdateAsync(AttendanceDto attendanceDto)
    {
        var attendance = await GetAttendanceFromIdAsync(attendanceDto.Id);
        
        ValidateAttendanceCreateDto(attendanceDto);
        
        attendance.Diagnosis = attendanceDto.Diagnosis;
        attendance.Remarks = attendanceDto.Remarks;
        attendance.Therapy = attendanceDto.Therapy;
        attendance.DateTime = attendanceDto.DateTime;
        attendance.PatientId = attendanceDto.PatientId;
        attendance.DoctorId = attendanceDto.DoctorId;
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);
        
        _repositoryManager.AttendanceRepository.Remove(attendance);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    private async Task<Attendance> GetAttendanceFromIdAsync(Guid id)
    {
        var attendance = await _repositoryManager.AttendanceRepository.FindByIdAsync(id);
        if (attendance is null)
            throw new AttendanceNotFoundException(id.ToString());
        
        return attendance;
    }

    private static void ValidateAttendanceCreateDto(AttendanceCreateDto dto)
    {
        if (dto.PatientId == Guid.Empty)
            throw new ArgumentException("Patient Id cannot be empty");
        if (dto.DoctorId == Guid.Empty)
            throw new ArgumentException("Doctor Id cannot be empty");
        if (dto.DateTime < DateTime.Now)
            throw new ArgumentException("DateTime cannot be in the past");
        
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Diagnosis, nameof(dto.Diagnosis));
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Remarks, nameof(dto.Remarks));
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Therapy, nameof(dto.Therapy));
    }
}