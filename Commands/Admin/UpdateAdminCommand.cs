using Dtos.Admin;
using MediatR;

namespace Commands.Admin
{
    public record UpdateAdminCommand(AdminDto Dto, Guid Id) : IRequest;
}
