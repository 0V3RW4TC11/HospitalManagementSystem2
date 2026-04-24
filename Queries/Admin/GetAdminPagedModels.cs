using MediatR;
using ViewModels.Admin;
using X.PagedList;

namespace Queries.Admin
{
    public record GetAdminPagedModels(int PageNumber, int PageSize) : IRequest<IPagedList<AdminIndexViewModel>>;
}
