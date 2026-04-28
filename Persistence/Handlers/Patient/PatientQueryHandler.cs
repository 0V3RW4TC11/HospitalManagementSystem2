using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using Persistence.Helpers;
using Queries.Patient;
using ViewModels.Patient;
using ViewModels.User;

namespace Persistence.Handlers.Patient
{
    public class PatientQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        PagedModelsQueryHandler<Entities.Patient, PatientIndexViewModel>(context, x => x.FirstName),
        IRequestHandler<GetPatientManageModel, ManageViewModel<PatientDataViewModel>>,
        IRequestHandler<GetPatientEditModel, EditViewModel<PatientDataViewModel>>
    {
        private readonly UserQueryHandlerHelper _helper = new(context, userManager);

        public async Task<ManageViewModel<PatientDataViewModel>> Handle(GetPatientManageModel request, CancellationToken cancellationToken)
        {
            return await _helper.CreateManageViewModelAsync<Entities.Patient, PatientDataViewModel>(request.Id, cancellationToken);
        }

        public async Task<EditViewModel<PatientDataViewModel>> Handle(GetPatientEditModel request, CancellationToken cancellationToken)
        {
            return await _helper.CreateEditViewModelAsync<Entities.Patient, PatientDataViewModel>(request.Id, cancellationToken);
        }
    }
}
