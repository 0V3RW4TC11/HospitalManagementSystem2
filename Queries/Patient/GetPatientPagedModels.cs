using MediatR;
using ViewModels.Patient;
using X.PagedList;

namespace Queries.Patient
{
    public record GetPatientPagedModels(int PageNumber, int PageSize) : IRequest<IPagedList<PatientIndexViewModel>>;
}
