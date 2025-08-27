namespace Services.Abstractions
{
    public interface IAccountService
    {
        Task LoginAsync(
            string username,
            string password,
            bool isPersistent,
            bool enableLockoutOnFail);

        Task LogoutAsync();

        Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);

        Task ResetPasswordAsync(Guid userId, string newPassword);

        Task<string> GetUserNameAsync(Guid userId);
    }
}
