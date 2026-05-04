using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using Persistence.Handlers.Helpers;
using Queries.Shared;
using ViewModels.Doctor;
using ViewModels.Shared;

namespace Persistence.Handlers.Doctor
{
    public class DoctorEditSpecViewQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        EditQueryHandlerBase<Entities.Doctor, DoctorSpecsView>(context, userManager)
    {
        public override async Task<EditUserViewModel<DoctorSpecsView>> Handle(GetEditUserModel<DoctorSpecsView> request, CancellationToken cancellationToken)
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
