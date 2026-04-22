using MediatR;
using ViewModels.Admin;

namespace Queries.Admin
{
    public record GetManageAdminModel(Guid Id) : IRequest<ManageViewModel>;
}
