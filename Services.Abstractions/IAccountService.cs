namespace Services.Abstractions
{
    public interface IAccountService
    {
        Task<string> GetIdentityIdFromUserId(Guid userId);

        Task<Guid> GetUserIdFromIdentityId(string identityId);
    }
}
