namespace Domain.Exceptions;

public class AdminBadRequest : BadRequestException
{
    public AdminBadRequest(string message) : base(message)
    {
    }
}