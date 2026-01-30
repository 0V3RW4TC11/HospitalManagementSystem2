using Dtos.Admin;
using MediatR;

namespace Commands.Admin.UpdateAdmin
{
    public record UpdateAdminCommand(AdminDto Dto, Guid Id) : IRequest;
}
