using MediatR;

namespace Commands.Admin
{
    public record CreateAdminCommand(AdminData Data, string Password) : IRequest;
}