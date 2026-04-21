using MediatR;
using ViewModels.Admin;

namespace Queries.Admin
{
    public record GetManageAdminQuery(Guid Id) : IRequest<ManageViewModel>;
}
