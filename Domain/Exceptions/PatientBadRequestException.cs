namespace Domain.Exceptions;

public class PatientBadRequestException : BadRequestException
{
    public PatientBadRequestException(string message) : base(message)
    {
    }
}