using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Services.Abstractions;

namespace Services.Helpers;

internal sealed class AccountHelper
{
    private readonly IRepositoryManager _repositoryManager;
    
    public AccountHelper(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task CreateAsync(Guid userId, string role, string username, string password)
    {
        var account = new Account
        {
            UserId = userId,
            IdentityUserId = await _repositoryManager.IdentityProvider.CreateAsync(username, password)
        };

        await _repositoryManager.IdentityProvider.AddToRoleAsync(account.IdentityUserId, role);

        _repositoryManager.AccountRepository.Add(account);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid userId)
    {
        var account = await _repositoryManager.AccountRepository.FindByUserIdAsync(userId)
            ?? throw new AccountNotFoundException("Account not found for User Id: " + userId);
        
        _repositoryManager.AccountRepository.Remove(account);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
        
        await _repositoryManager.IdentityProvider.RemoveAsync(account.IdentityUserId);
    }
}