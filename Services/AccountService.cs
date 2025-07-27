using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Providers;
using Domain.Repositories;
using Services.Abstractions;

namespace Services;

internal sealed class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public AccountService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task CreateAsync(Guid userId, string role, string username, string password)
    {
        var account = new Account
        {
            UserId = userId,
            IdentityUserId = await _unitOfWork.IdentityProvider.CreateAsync(username, password)
        };

        await _unitOfWork.IdentityProvider.AddToRoleAsync(account.IdentityUserId, role);

        _unitOfWork.AccountRepository.Add(account);
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Guid> FindUserIdByIdentityIdAsync(string identityId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identityId, nameof(identityId));
        
        var account = await _unitOfWork.AccountRepository.FindByIdentityIdAsync(identityId);
        if (account is null)
            throw AccountNotFoundException.ForIdentityId(identityId);
        
        return account.UserId;
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var account = await _unitOfWork.AccountRepository.FindByUserIdAsync(userId);
        if (account is null)
            throw AccountNotFoundException.ForUserId(userId);
        
        _unitOfWork.AccountRepository.Remove(account);
        
        await _unitOfWork.SaveChangesAsync();
        
        await _unitOfWork.IdentityProvider.RemoveAsync(account.IdentityUserId);
    }
}