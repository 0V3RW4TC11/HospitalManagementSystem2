using Ardalis.Specification;
using Mapster;
using Services.Dtos.Attendance;

namespace Services.Specifications.Attendance
{
    internal class DoctorAttendanceSearchResultSpec : Specification<Domain.Entities.Attendance, DoctorAttendanceSearchResultDto>
    {
        public DoctorAttendanceSearchResultSpec(Guid doctorId)
        {
            Query
                .Select(a => new() { AttendanceId = a.Id, PatientId = a.PatientId, DateTime = a.DateTime })
                .Where(a => a.DoctorId == doctorId);
        }
    }
}
