namespace Services.Abstractions;

public interface IAccountService
{
    Task CreateAsync(Guid userId, string role, string username, string password);
    
    Task<Guid> FindUserIdByIdentityIdAsync(string identityId);
    
    Task DeleteByUserIdAsync(Guid userId);
}