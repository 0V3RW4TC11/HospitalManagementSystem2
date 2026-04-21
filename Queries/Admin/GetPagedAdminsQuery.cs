using MediatR;
using ViewModels.Admin;
using X.PagedList;

namespace Queries.Admin
{
    public record GetPagedAdminsQuery(int PageNumber, int PageSize) : IRequest<IPagedList<IndexViewModel>>;
}
