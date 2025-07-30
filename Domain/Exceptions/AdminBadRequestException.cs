namespace Domain.Exceptions;

public class AdminBadRequestException : BadRequestException
{
    public AdminBadRequestException(string message) : base(message)
    {
    }
}