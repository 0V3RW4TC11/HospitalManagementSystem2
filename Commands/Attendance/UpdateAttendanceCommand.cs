namespace Commands.Attendance
{
    public record UpdateAttendanceCommand(Guid Id) : CreateAttendanceCommand;
}
