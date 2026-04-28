using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using Persistence.Helpers;
using Queries.Admin;
using ViewModels.Admin;
using ViewModels.User;

namespace Persistence.Handlers.Admin
{
    public class AdminQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) : 
        PagedModelsQueryHandler<Entities.Admin, AdminIndexViewModel>(context, x => x.FirstName),
        IRequestHandler<GetAdminManageModel, ManageViewModel<AdminDataViewModel>>,
        IRequestHandler<GetAdminEditModel, EditViewModel<AdminDataViewModel>>
    {
        private readonly UserQueryHandlerHelper _helper = new(context, userManager);

        public async Task<ManageViewModel<AdminDataViewModel>> Handle(GetAdminManageModel request, CancellationToken cancellationToken)
        {
            return await _helper.CreateManageViewModelAsync<Entities.Admin,AdminDataViewModel>(request.Id, cancellationToken);
        }

        public async Task<EditViewModel<AdminDataViewModel>> Handle(GetAdminEditModel request, CancellationToken cancellationToken)
        {
            return await _helper.CreateEditViewModelAsync<Entities.Admin, AdminDataViewModel>(request.Id, cancellationToken);
        }
    }
}
