using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;
using Services.Dtos.Attendance;
using System.Linq.Expressions;

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
        await ValidateAttendanceCreateDto(attendanceCreateDto);
        
        var attendance = attendanceCreateDto.Adapt<Attendance>();
        attendance.DateTime = DateTime.UtcNow;

        _repositoryManager.AttendanceRepository.Add(attendance);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);

        _repositoryManager.AttendanceRepository.Remove(attendance);

        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task<(DoctorAttendanceSearchResultDto[] List, int TotalCount)> FindAttendancesByDoctorPagedAsync(Guid doctorId, int pageNumber, int pageSize)
    {
        Expression<Func<Attendance, bool>> filter = a => a.DoctorId == doctorId;
        Expression<Func<Attendance, DoctorAttendanceSearchResultDto>> selector = a => new() { AttendanceId = a.Id, PatientId = a.PatientId, DateTime = a.DateTime };

        return await _repositoryManager.AttendanceRepository.GetPagedListAsync(filter, selector, pageNumber, pageSize);
    }

    public async Task<(PatientAttendanceSearchResultDto[] List, int TotalCount)> FindAttendancesByPatientPagedAsync(Guid patientId, int pageNumber, int pageSize)
    {
        Expression<Func<Attendance, bool>> filter = a => a.PatientId == patientId;
        Expression<Func<Attendance, PatientAttendanceSearchResultDto>> selector = a => new() { AttendanceId = a.Id, DoctorId = a.DoctorId, DateTime = a.DateTime };

        return await _repositoryManager.AttendanceRepository.GetPagedListAsync(filter, selector, pageNumber, pageSize);
    }

    public async Task<AttendanceDto> GetByIdAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);
        
        return attendance.Adapt<AttendanceDto>();
    }

    public async Task UpdateAsync(AttendanceDto attendanceDto)
    {
        var attendance = await GetAttendanceFromIdAsync(attendanceDto.Id);
        
        await ValidateAttendanceCreateDto(attendanceDto);
        
        attendance.Diagnosis = attendanceDto.Diagnosis;
        attendance.Remarks = attendanceDto.Remarks;
        attendance.Therapy = attendanceDto.Therapy;
        attendance.DateTime = attendanceDto.DateTime;
        attendance.PatientId = attendanceDto.PatientId;
        attendance.DoctorId = attendanceDto.DoctorId;
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }
    private static void ValidateAttendanceDetails(AttendanceCreateDto dto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Diagnosis, nameof(dto.Diagnosis));
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Remarks, nameof(dto.Remarks));
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Therapy, nameof(dto.Therapy));
    }

    private async Task<Attendance> GetAttendanceFromIdAsync(Guid id)
    {
        var attendance = await _repositoryManager.AttendanceRepository.FindByIdAsync(id);
        if (attendance is null)
            throw new AttendanceNotFoundException();
        
        return attendance;
    }

    private async Task ValidateAttendanceCreateDto(AttendanceCreateDto dto)
    {
        await ValidateNotDuplicateAsync(dto);
        await ValidateAttendanceDto(dto);
    }

    private async Task ValidateAttendanceDto(AttendanceCreateDto dto)
    {
        ValidateAttendanceDetails(dto);
        await ValidateDoctorIdAsync(dto.DoctorId);
        await ValidatePatientIdAsync(dto.PatientId);
    }

    private async Task ValidateDoctorIdAsync(Guid id)
    {
        var exists = await _repositoryManager.DoctorRepository.ExistsAsync(d => d.Id == id);
        if (!exists)
            throw new DoctorNotFoundException();
    }

    private async Task ValidateNotDuplicateAsync(AttendanceCreateDto dto)
    {
        var exists = await _repositoryManager.AttendanceRepository.ExistsAsync(a =>
            a.DateTime == dto.DateTime && 
            a.DoctorId == dto.DoctorId && 
            a.PatientId == dto.PatientId);
        
        if (exists)
            throw new AttendanceDuplicationException("An Attendance with the same details already exists.");
    }

    private async Task ValidatePatientIdAsync(Guid id)
    {
        var exists = await _repositoryManager.PatientRepository.ExistsAsync(p => p.Id == id);
        if (!exists)
            throw new PatientNotFoundException();
    }
}