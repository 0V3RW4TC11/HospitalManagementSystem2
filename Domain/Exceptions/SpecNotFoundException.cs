namespace Domain.Exceptions;

public class SpecNotFoundException : NotFoundException
{
    public SpecNotFoundException(string id) : base($"Specialization not found for Id {id}.")
    {
    }
}