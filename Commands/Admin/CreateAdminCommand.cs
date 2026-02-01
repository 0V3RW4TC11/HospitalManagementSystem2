using Dtos.Admin;
using MediatR;

namespace Commands.Admin
{
    public record CreateAdminCommand(AdminDto Dto, string Password) : IRequest;
}
