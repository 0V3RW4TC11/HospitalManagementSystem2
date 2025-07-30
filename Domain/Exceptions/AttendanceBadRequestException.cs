namespace Domain.Exceptions;

public class AttendanceBadRequestException : BadRequestException
{
    public AttendanceBadRequestException(string message) : base(message)
    {
    }
}