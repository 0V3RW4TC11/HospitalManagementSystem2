namespace Abstractions;

public interface IIdentityProvider
{
    Task CreateIdentityAsync(Guid hmsUserId, string userName, string password, string role, CancellationToken cancellationToken);

    Task DeleteIdentityAsync(Guid hmsUserId, CancellationToken cancellationToken);
}