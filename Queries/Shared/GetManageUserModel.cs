using MediatR;
using ViewModels.Shared;

namespace Queries.Shared
{
    public record GetManageUserModel<TDataModel>(Guid Id) : IRequest<ManageUserViewModel<TDataModel>>
        where TDataModel : class;
}
