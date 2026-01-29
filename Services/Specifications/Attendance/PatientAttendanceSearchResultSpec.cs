using Ardalis.Specification;
using Services.Dtos.Attendance;

namespace Services.Specifications.Attendance
{
    internal class PatientAttendanceSearchResultSpec : Specification<Domain.Entities.Attendance, PatientAttendanceSearchResultDto>
    {
        public PatientAttendanceSearchResultSpec(Guid patientId)
        {
            Query
                .Select(a => new() { AttendanceId = a.Id, DoctorId = a.DoctorId, DateTime = a.DateTime })
                .Where(a => a.PatientId == patientId);
        }
    }
}
