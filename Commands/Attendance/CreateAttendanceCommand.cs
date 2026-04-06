using MediatR;

namespace Commands.Attendance
{
    public record CreateAttendanceCommand(AttendanceData Data) : IRequest;
}