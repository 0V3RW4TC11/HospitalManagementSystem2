namespace Domain.Exceptions;

public class DoctorBadRequest : BadRequestException
{
    public DoctorBadRequest(string message) : base(message)
    {
    }
}