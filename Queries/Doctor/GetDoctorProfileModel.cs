using MediatR;
using ViewModels.Doctor;

namespace Queries.Doctor
{
    public record GetDoctorProfileModel(Guid Id) : IRequest<ProfileDoctorViewModel>;
}
