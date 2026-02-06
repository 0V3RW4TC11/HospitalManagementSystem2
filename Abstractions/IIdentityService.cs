namespace Abstractions;

public interface IIdentityService
{
    Task CreateIdentityAsync(Guid id, string userName, string password, string role, CancellationToken cancellationToken);

    Task DeleteIdentityAsync(Guid id, CancellationToken cancellationToken);
}