using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Helpers;
using Queries.Patient;
using ViewModels.Patient;
using ViewModels.User;
using X.PagedList;

namespace Persistence.Handlers.Patient
{
    public class PatientQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        IRequestHandler<GetPatientPagedModels, IPagedList<PatientIndexViewModel>>,
        IRequestHandler<GetPatientManageModel, ManageViewModel<PatientDataViewModel>>,
        IRequestHandler<GetPatientEditModel, EditViewModel<PatientDataViewModel>>
    {
        private readonly UserQueryHandlerHelper _helper = new(context, userManager);

        public async Task<IPagedList<PatientIndexViewModel>> Handle(GetPatientPagedModels request, CancellationToken cancellationToken)
        {
            return await _helper.CreatePagedModelsAsync<Entities.Patient, PatientIndexViewModel>(
                a => a.FirstName,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }

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
