using MediatR;
using ViewModels.Patient;
using ViewModels.User;

namespace Queries.Patient
{
    public record GetPatientManageModel(Guid Id) : IRequest<ManageViewModel<PatientDataViewModel>>;
}
