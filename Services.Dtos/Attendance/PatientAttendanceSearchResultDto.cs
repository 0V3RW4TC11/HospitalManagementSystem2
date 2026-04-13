namespace Services.Dtos.Attendance
{
    public class PatientAttendanceSearchResultDto
    {
        public Guid AttendanceId { get; set; }

        public Guid DoctorId { get; set; }

        public DateTime DateTime { get; set; }
    }
}
