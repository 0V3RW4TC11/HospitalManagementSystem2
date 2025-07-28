namespace Domain.Exceptions;

public class PatientNotFoundException : NotFoundException
{
    public PatientNotFoundException(string id) : base($"Patient not found for Id: {id}.")
    {
    }
}