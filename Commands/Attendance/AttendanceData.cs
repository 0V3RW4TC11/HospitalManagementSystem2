namespace Commands.Attendance
{
    public record AttendanceData(
        Guid PatientId,
        Guid DoctorId,
        DateTime DateTime,
        string Diagnosis,
        string Remarks,
        string Therapy);
}
