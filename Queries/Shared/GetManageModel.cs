using MediatR;
using ViewModels.User;

namespace Queries.Shared
{
    public record GetManageModel<TDataModel>(Guid Id) : IRequest<ManageViewModel<TDataModel>>
        where TDataModel : class;
}
