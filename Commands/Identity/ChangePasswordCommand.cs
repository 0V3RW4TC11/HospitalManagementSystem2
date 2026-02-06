using MediatR;

namespace Commands.Identity
{
    public record ChangePasswordCommand(Guid HmsUserId, string OldPassword, string NewPassword) : IRequest;
}
