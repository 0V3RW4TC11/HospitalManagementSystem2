using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Services.Abstractions;

namespace Services;

internal sealed class AccountService : IAccountService
{
    private readonly IRepositoryManager _repositoryManager;
    
    public AccountService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

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

    public async Task<Guid> FindUserIdByIdentityIdAsync(string identityId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identityId, nameof(identityId));
        
        var account = await _repositoryManager.AccountRepository.FindByIdentityIdAsync(identityId);
        if (account is null)
            throw AccountNotFoundException.ForIdentityId(identityId);
        
        return account.UserId;
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var account = await _repositoryManager.AccountRepository.FindByUserIdAsync(userId);
        if (account is null)
            throw AccountNotFoundException.ForUserId(userId);
        
        _repositoryManager.AccountRepository.Remove(account);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
        
        await _repositoryManager.IdentityProvider.RemoveAsync(account.IdentityUserId);
    }
}