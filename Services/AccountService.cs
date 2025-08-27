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
            var account = await GetAccountByUserIdAsync(userId);
            await _repositoryManager.IdentityProvider.ChangePasswordAsync(
                account.IdentityUserId,
                oldPassword,
                newPassword);
        }

        public async Task ResetPasswordAsync(Guid userId, string newPassword)
        {
            var account = await GetAccountByUserIdAsync(userId);
            await _repositoryManager.IdentityProvider.ResetPasswordAsync(
                account.IdentityUserId,
                newPassword);
        }

        public async Task<string> GetUserNameAsync(Guid userId)
        {
            var account = await GetAccountByUserIdAsync(userId);
            return await _repositoryManager.IdentityProvider.GetUserNameAsync(account.IdentityUserId);
        }

        private async Task<Account> GetAccountByUserIdAsync(Guid userId)
        {
            return await _repositoryManager.AccountRepository.FindByUserIdAsync(userId)
                ?? throw new AccountNotFoundException("Account not found for User Id: " + userId);
        }
    }
}
