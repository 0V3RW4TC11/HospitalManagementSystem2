using MediatR;
using ViewModels.Patient;
using X.PagedList;

namespace Queries.Patient
{
    public record GetPagedPatientsQuery(int PageNumber, int PageSize) : IRequest<IPagedList<IndexViewModel>>;
}
