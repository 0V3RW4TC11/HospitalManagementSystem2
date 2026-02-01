namespace Abstractions;

public interface IIdentityProvider
{
    IdentityManager IdentityManager { get; }
}