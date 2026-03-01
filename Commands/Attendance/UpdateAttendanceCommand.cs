using MediatR;

namespace Commands.Attendance
{
    public record UpdateAttendanceCommand(
        Guid Id,
        Guid PatientId,
        Guid DoctorId,
        DateTime DateTime,
        string Diagnosis,
        string Remarks,
        string Therapy) : IRequest;
}