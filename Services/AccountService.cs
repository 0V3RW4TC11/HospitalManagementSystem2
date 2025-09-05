using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Services.Abstractions;

namespace Services
{
    internal class AccountService : IAccountService
    {
        private readonly IRepositoryManager _repositoryManager;

        public AccountService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task LoginAsync(
            string username, 
            string password, 
            bool isPersistent, 
            bool enableLockoutOnFail)
        {
            await _repositoryManager.IdentityProvider.LoginAsync(
                username,
                password, 
                isPersistent, 
                enableLockoutOnFail);
        }

        public async Task LogoutAsync()
        {
            await _repositoryManager.IdentityProvider.LogoutAsync();
        }

        public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            await _repositoryManager.IdentityProvider.ChangePasswordAsync(
                await GetIdentityIdByUserIdAsync(userId),
                oldPassword,
                newPassword);
        }

        public async Task ResetPasswordAsync(Guid userId, string newPassword)
        {
            await _repositoryManager.IdentityProvider.ResetPasswordAsync(
                await GetIdentityIdByUserIdAsync(userId),
                newPassword);
        }

        public async Task SetLockoutAsync(Guid userId, bool enabled)
        {
            await _repositoryManager.IdentityProvider.SetLockoutAsync(
                await GetIdentityIdByUserIdAsync(userId),
                enabled);
        }

        public async Task<bool> IsLockedOut(Guid userId)
        {
            return await _repositoryManager.IdentityProvider.IsLockedOut(
                await GetIdentityIdByUserIdAsync(userId));
        }

        public async Task<string> GetUserNameAsync(Guid userId)
        {
            return await _repositoryManager.IdentityProvider.GetUserNameAsync(
                await GetIdentityIdByUserIdAsync(userId));
        }

        private async Task<string> GetIdentityIdByUserIdAsync(Guid userId)
        {
            var account = await GetAccountAsync(userId);
            return account.IdentityUserId;
        }

        private async Task<Account> GetAccountAsync(Guid userId)
        {
            return await _repositoryManager.AccountRepository.FindByUserIdAsync(userId)
                ?? throw new AccountNotFoundException("Account not found for User Id: " + userId);
        }
    }
}
