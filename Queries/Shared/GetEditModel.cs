using MediatR;
using ViewModels.User;

namespace Queries.Shared
{
    public record GetEditModel<TDataModel>(Guid Id) : IRequest<EditViewModel<TDataModel>>
        where TDataModel : class;
}
