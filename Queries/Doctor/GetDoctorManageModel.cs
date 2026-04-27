using MediatR;
using ViewModels.Doctor;

namespace Queries.Doctor
{
    public record GetDoctorManageModel(Guid Id) : IRequest<ManageDoctorViewModel>;
}
