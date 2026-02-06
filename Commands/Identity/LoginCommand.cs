using MediatR;

namespace Commands.Identity
{
    public record LoginCommand(
        string UserName,
        string Password,
        bool IsPersistent,
        bool EnableLockoutOnFail) : IRequest;
}
