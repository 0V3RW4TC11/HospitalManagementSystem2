namespace Services.Abstractions
{
    public interface IIdentityService
    {
        Task LoginAsync(
            string username,
            string password,
            bool isPersistent,
            bool enableLockoutOnFail);

        Task LogoutAsync();

        Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);

        Task ResetPasswordAsync(Guid userId, string newPassword);

        Task SetLockoutAsync(Guid userId, bool enabled);

        Task<bool> IsLockedOut(Guid userId);

        Task<string> GetUserNameAsync(Guid userId);

        Task<Guid> GetLoggedInUserId();
    }
}
