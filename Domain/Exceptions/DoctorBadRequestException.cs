namespace Domain.Exceptions;

public class DoctorBadRequestException : BadRequestException
{
    public DoctorBadRequestException(string message) : base(message)
    {
    }
}