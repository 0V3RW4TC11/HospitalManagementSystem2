using MediatR;
using ViewModels.Doctor;
using X.PagedList;

namespace Queries.Doctor
{
    public record GetDoctorPagedModels(int PageNumber, int PageSize) : IRequest<IPagedList<DoctorIndexViewModel>>;
}
