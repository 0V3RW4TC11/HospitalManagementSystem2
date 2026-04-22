using MediatR;
using ViewModels.Admin;
using ViewModels.User;

namespace Queries.Admin
{
    public record GetManageAdminModel(Guid Id) : IRequest<ManageViewModel<DataViewModel>>;
}
