using MediatR;
using ViewModels.Patient;
using ViewModels.User;

namespace Queries.Patient
{
    public record GetPatientEditModel(Guid Id) : IRequest<EditViewModel<PatientDataViewModel>>;
}
