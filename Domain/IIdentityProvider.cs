namespace Domain;

public interface IIdentityProvider
{
    Task<string> CreateAsync(string username, string password);
    
    Task RemoveAsync(string identityId);

    Task CreateRoleAsync(string roleName);

    Task RemoveRoleAsync(string roleName);
    
    Task AddToRoleAsync(string identityId, string role);

    Task LoginAsync(
        string username,
        string password,
        bool isPersistent,
        bool enableLockoutOnFail);

    Task LogoutAsync();

    Task ChangePasswordAsync(string identityId, string oldPassword, string newPassword);

    Task ResetPasswordAsync(string identityId, string newPassword);

    Task<string> GetUserNameAsync(string identityId);

    Task<bool> EmailExistsAsync(string email);

    Task<bool> IsLockedOut(string identityId);

    Task SetLockoutAsync(string identityId, bool enabled);

    string GetLoggedInUserIdentityId();
}