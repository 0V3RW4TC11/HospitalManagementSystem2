using MediatR;

namespace Commands.Attendance
{
    public record UpdateAttendanceCommand(Guid Id, AttendanceData Data) : IRequest;
}