namespace Abstractions;

public interface IIdentityService
{
    Task CreateIdentityAsync(Guid hmsUserId, string userName, string password, string role, CancellationToken cancellationToken);

    Task DeleteIdentityAsync(Guid hmsUserId, CancellationToken cancellationToken);
}