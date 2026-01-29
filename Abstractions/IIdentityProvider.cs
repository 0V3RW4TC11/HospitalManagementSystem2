using Domain.Entities;

namespace Abstractions;

public interface IIdentityProvider
{
    Task CreateAsync(Admin admin, string password, CancellationToken cancellationToken);

    Task CreateAsync(Doctor doctor, string password, CancellationToken cancellationToken);

    Task CreateAsync(Patient patient, string password, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}