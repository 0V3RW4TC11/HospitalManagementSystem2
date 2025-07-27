namespace Domain.Exceptions;

public class AttendanceNotFoundException : NotFoundException
{
    public AttendanceNotFoundException(string id) : base($"Attendance not found for Id: {id}.")
    {
    }
}