using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Queries.Patient;
using ViewModels.Patient;
using X.PagedList;
using X.PagedList.EF;

namespace Persistence.Handlers.Patient
{
    public class PatientQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        IRequestHandler<GetPagedPatientsQuery, IPagedList<IndexViewModel>>
    {
        public async Task<IPagedList<IndexViewModel>> Handle(GetPagedPatientsQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await context.Patients.CountAsync(cancellationToken);
            return await context.Patients
                .OrderBy(p => p.FirstName)
                .ProjectToType<IndexViewModel>()
                .ToPagedListAsync(request.PageNumber, request.PageSize, totalCount);
        }
    }
}
