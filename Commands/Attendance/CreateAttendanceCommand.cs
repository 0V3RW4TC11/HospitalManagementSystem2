using MediatR;

namespace Commands.Attendance
{
    public record CreateAttendanceCommand(
        Guid PatientId,
        Guid DoctorId,
        DateTime DateTime,
        string Diagnosis,
        string Remarks,
        string Therapy) : IRequest;
}