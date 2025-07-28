namespace Domain.Exceptions;

public class DoctorNotFoundException : NotFoundException
{
    public DoctorNotFoundException(string id) : base($"Doctor not found for Id: {id}.")
    {
    }
}