using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Handlers.Base;
using Queries.Shared;
using ViewModels.Doctor;
using ViewModels.Shared;
using ViewModels.Specialization;

namespace Persistence.Handlers.Doctor
{
    public class DoctorEditSpecEditQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        EditQueryHandlerBase<Entities.Doctor, DoctorSpecsEdit>(context, userManager)
    {
        public override async Task<EditUserViewModel<DoctorSpecsEdit>> Handle(GetEditUserModel<DoctorSpecsEdit> request, CancellationToken cancellationToken)
        {
            var model = await base.Handle(request, cancellationToken);
            
            var specializations = await Context.Set<Entities.DoctorSpecialization>()
                .Where(ds => ds.DoctorId == request.Id)
                .Join(
                    Context.Set<Entities.Specialization>(),
                    ds => ds.SpecializationId,
                    s => s.Id,
                    (ds, s) => s)
                .ProjectToType<SpecViewModel>()
                .ToListAsync(cancellationToken);

            var dsJson = new DoctorSpecializationsJson();
            dsJson.SetJsonFromSpecializations(specializations);
            model.Data.SpecializationsJson = dsJson;

            return model;
        }
    }
}
