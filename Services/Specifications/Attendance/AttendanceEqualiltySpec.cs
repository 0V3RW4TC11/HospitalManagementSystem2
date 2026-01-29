using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications.Attendance
{
    internal class AttendanceEqualiltySpec : Specification<Domain.Entities.Attendance>
    {
        public AttendanceEqualiltySpec(
            Guid doctorId,
            Guid patientId,
            DateTime minDate,
            DateTime maxDate)
        {
            Query.Where(
                a =>
                a.DoctorId == doctorId &&
                a.PatientId == patientId &&
                a.DateTime >= minDate &&
                a.DateTime <= maxDate);
        }
    }
}
