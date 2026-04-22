using MediatR;
using ViewModels.Admin;
using ViewModels.User;

namespace Queries.Admin
{
    public record GetEditAdminModel(Guid Id) : IRequest<EditViewModel<DataViewModel>>;
}
