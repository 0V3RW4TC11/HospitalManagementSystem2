namespace Domain.Exceptions;

public class AdminNotFoundException : NotFoundException
{
    public AdminNotFoundException(string id) : base($"Admin not found for Id: {id}.")
    {
    }
}