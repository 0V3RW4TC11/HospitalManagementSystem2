using DataTransfer.Attendance;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;
using Services.Abstractions;

namespace Services;

internal sealed class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;

    public AttendanceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(AttendanceCreateDto attendanceCreateDto)
    {
        ValidateAttendanceCreateDto(attendanceCreateDto);
        
        var attendance = attendanceCreateDto.Adapt<Attendance>();
        
        _unitOfWork.AttendanceRepository.Add(attendance);
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<AttendanceDto>> GetAllByPatientIdAsync(Guid id)
    {
        var attendances = await _unitOfWork.AttendanceRepository.GetAllByPatientIdAsync(id);
        
        return attendances.Adapt<IEnumerable<AttendanceDto>>();
    }

    public async Task<IEnumerable<AttendanceDto>> GetAllByDoctorIdAsync(Guid id)
    {
        var attendances = await _unitOfWork.AttendanceRepository.GetAllByDoctorIdAsync(id);
        
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
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);
        
        _unitOfWork.AttendanceRepository.Remove(attendance);
        
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Attendance> GetAttendanceFromIdAsync(Guid id)
    {
        var attendance = await _unitOfWork.AttendanceRepository.FindByIdAsync(id);
        if (attendance is null)
            throw new AttendanceNotFoundException(id.ToString());
        
        return attendance;
    }

    private void ValidateAttendanceCreateDto(AttendanceCreateDto dto)
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