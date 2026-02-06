using MediatR;

namespace Commands.Identity
{
    public record ResetPasswordCommand(Guid HmsUserId, string NewPassword) : IRequest;
}
