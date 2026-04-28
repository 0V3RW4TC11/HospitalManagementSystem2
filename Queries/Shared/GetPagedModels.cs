using MediatR;
using X.PagedList;

namespace Queries.Shared
{
    public record GetPagedModels<T>(int PageNumber, int PageSize) : IRequest<IPagedList<T>>;
}
