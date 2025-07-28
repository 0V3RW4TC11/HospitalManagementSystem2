namespace Domain.Exceptions;

public class AccountNotFoundException : NotFoundException
{
    public AccountNotFoundException(string message) : base(message)
    {
    }

    public static AccountNotFoundException ForUserId(Guid userId)
    {
        var message = $"Account not found for User Id: {userId.ToString()}.";
        return new AccountNotFoundException(message);
    }
    
    public static AccountNotFoundException ForIdentityId(string identityId)
    {
        var message = $"Account not found for Identity Id: {identityId}.";
        return new AccountNotFoundException(message);
    }
}