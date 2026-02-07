using MediatR;

namespace Commands.Identity
{
    public record ChangePasswordCommand(Guid HmsUserId, string CurrentPassword, string NewPassword) : IRequest;
}
