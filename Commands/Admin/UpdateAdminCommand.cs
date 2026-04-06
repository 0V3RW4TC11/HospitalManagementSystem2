using MediatR;

namespace Commands.Admin
{
    public record UpdateAdminCommand(Guid Id, AdminData Data) : IRequest;
}