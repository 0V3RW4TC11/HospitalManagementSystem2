namespace Services.Abstractions;

public interface IAccountManager
{
    Task CreateAsync(Guid userId, string role, string username, string password);
    
    Task DeleteAsync(Guid userId);
}