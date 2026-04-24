using MediatR;
using ViewModels.Admin;
using ViewModels.User;

namespace Queries.Admin
{
    public record GetAdminManageModel(Guid Id) : IRequest<ManageViewModel<AdminDataViewModel>>;
}
