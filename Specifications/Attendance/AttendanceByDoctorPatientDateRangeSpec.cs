using Ardalis.Specification;

namespace Specifications.Attendance
{
    public class AttendanceByDoctorPatientDateRangeSpec : SingleResultSpecification<Entities.Attendance>
    {
        public AttendanceByDoctorPatientDateRangeSpec(Guid doctorId, Guid patientId, DateTime dateMin, DateTime dateMax)
        {
            Query.Where(a =>
                a.DoctorId == doctorId &&
                a.PatientId == patientId &&
                a.DateTime >= dateMin &&
                a.DateTime <= dateMax);
        }
    }
}
