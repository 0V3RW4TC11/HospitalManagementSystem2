using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using Persistence.Handlers.Helpers;
using Queries.Shared;
using ViewModels.Doctor;
using ViewModels.Shared;

namespace Persistence.Handlers.Doctor
{
    public class DoctorManageQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        ManageQueryHandlerBase<Entities.Doctor, DoctorSpecsView>(context, userManager)
    {
        public async override Task<ManageUserViewModel<DoctorSpecsView>> Handle(
            GetManageUserModel<DoctorSpecsView> request, CancellationToken cancellationToken)
        {
            var model = await base.Handle(request, cancellationToken);
            model.Data.SpecializationNames = await DoctorQueryHelper.GetSpecializationNamesByDoctorId(
                Context,
                request.Id,
                cancellationToken);
            return model;
        }
    }
}
