namespace Domain.Providers;

public interface IIdentityProvider
{
    Task<string> CreateAsync(string username, string password);
    
    Task RemoveAsync(string identityId);
    
    Task AddToRoleAsync(string identityId, string role);

    Task AddRolesAsync(string[] roles);
    
    Task<bool> EmailExistsAsync(string email);
}