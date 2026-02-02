namespace Abstractions;

public interface IIdentityService
{
    Task CreateIdentityAsync(Guid id, string userName, string password, string role, CancellationToken ct);

    Task DeleteIdentityAsync(Guid id, CancellationToken cancellationToken);
}