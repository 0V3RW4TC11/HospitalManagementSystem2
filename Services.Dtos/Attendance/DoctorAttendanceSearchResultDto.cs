namespace Services.Dtos.Attendance
{
    public class DoctorAttendanceSearchResultDto
    {
        public Guid AttendanceId { get; set; }

        public Guid PatientId { get; set; }

        public DateTime DateTime { get; set; }
    }
}
