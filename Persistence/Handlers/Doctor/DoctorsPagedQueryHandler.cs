using Persistence.Handlers.Base;
using ViewModels.Doctor;

namespace Persistence.Handlers.Doctor
{
    public class DoctorsPagedQueryHandler(HmsDbContext context) :
        PagedQueryHandlerBase<Entities.Doctor, DoctorIndexViewModel>(context, x => x.FirstName);
}
