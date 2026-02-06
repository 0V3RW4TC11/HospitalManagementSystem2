using MediatR;

namespace Commands.Attendance
{
    public record DeleteAttendanceCommand(Guid Id) : IRequest;
}