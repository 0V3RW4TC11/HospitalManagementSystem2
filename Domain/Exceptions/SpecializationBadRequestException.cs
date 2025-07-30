namespace Domain.Exceptions;

public class SpecializationBadRequestException : BadRequestException
{
    public SpecializationBadRequestException(string message) : base(message)
    {
    }
}