using Dtos.Admin;
using MediatR;

namespace Commands.Admin.CreateAdmin
{
    public record CreateAdminCommand(AdminDto Dto, string Password) : IRequest;
}
