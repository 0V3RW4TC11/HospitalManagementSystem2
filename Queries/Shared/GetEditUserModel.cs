using MediatR;
using ViewModels.Shared;

namespace Queries.Shared
{
    public record GetEditUserModel<TDataModel>(Guid Id) : IRequest<EditUserViewModel<TDataModel>>
        where TDataModel : class;
}
