using MediatR;

namespace Commands.Identity
{
    public record SetLockOutCommand(Guid HmsUserId, bool Enabled) : IRequest;
}
