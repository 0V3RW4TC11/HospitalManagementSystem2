using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using ViewModels.Patient;

namespace Persistence.Handlers.Patient
{
    public class PatientUserQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        UserQueryHandlerBase<Entities.Patient, PatientDataViewModel>(context, userManager);
}
