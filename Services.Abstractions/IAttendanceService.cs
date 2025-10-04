using Services.Dtos.Attendance;

namespace Services.Abstractions;

public interface IAttendanceService
{
    Task CreateAsync(AttendanceCreateDto attendanceCreateDto);

    Task DeleteAsync(Guid id);

    Task<(DoctorAttendanceSearchResultDto[] List, int TotalCount)> FindAttendancesByDoctorPagedAsync(Guid doctorId, int pageNumber, int pageSize);

    Task<(PatientAttendanceSearchResultDto[] List, int TotalCount)> FindAttendancesByPatientPagedAsync(Guid patientId, int pageNumber, int pageSize);

    Task<AttendanceDto> GetByIdAsync(Guid id);
    
    Task UpdateAsync(AttendanceDto attendanceDto);
}