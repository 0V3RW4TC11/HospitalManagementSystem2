namespace Services.Abstractions
{
    public interface IAccountDictionary
    {
        Task<string> GetIdentityIdByUserId(Guid userId);

        Task<Guid> GetUserIdByIdentityId(string identityId);
    }
}
