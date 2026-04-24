using MediatR;
using ViewModels.Admin;
using ViewModels.User;

namespace Queries.Admin
{
    public record GetAdminEditModel(Guid Id) : IRequest<EditViewModel<AdminDataViewModel>>;
}
