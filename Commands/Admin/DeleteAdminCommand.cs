using MediatR;

namespace Commands.Admin
{
    public record DeleteAdminCommand(Guid Id) : IRequest;
}