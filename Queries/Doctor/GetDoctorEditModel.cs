using MediatR;
using ViewModels.Doctor;

namespace Queries.Doctor
{
    public record GetDoctorEditModel(Guid Id) : IRequest<EditDoctorViewModel>;
}
