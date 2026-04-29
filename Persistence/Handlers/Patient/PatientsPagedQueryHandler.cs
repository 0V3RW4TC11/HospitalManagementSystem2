using Persistence.Handlers.Base;
using ViewModels.Patient;

namespace Persistence.Handlers.Patient
{
    public class PatientsPagedQueryHandler(HmsDbContext context) :
        PagedQueryHandlerBase<Entities.Patient, PatientIndexViewModel>(context, x => x.FirstName);
}
