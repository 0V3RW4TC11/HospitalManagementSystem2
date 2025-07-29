namespace Domain.Exceptions;

public class PatientBadRequest : BadRequestException
{
    public PatientBadRequest(string message) : base(message)
    {
    }
}