using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using ViewModels.Patient;

namespace Persistence.Handlers.Patient
{
    public class PatientEditQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        EditQueryHandlerBase<Entities.Patient, PatientDataViewModel>(context, userManager);
}
