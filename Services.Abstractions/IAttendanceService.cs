using DataTransfer.Attendance;

namespace Services.Abstractions;

public interface IAttendanceService
{
    Task CreateAsync(AttendanceCreateDto attendanceCreateDto);
    
    Task<IEnumerable<AttendanceShortDto>> GetAllByPatientIdAsync(Guid id);
    
    Task<IEnumerable<AttendanceShortDto>> GetAllByDoctorIdAsync(Guid id);
    
    Task<AttendanceDto> GetByIdAsync(Guid id);
    
    Task UpdateAsync(AttendanceDto attendanceDto);
    
    Task DeleteAsync(Guid id);
}