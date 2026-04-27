using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Queries.Specialization;
using ViewModels.Specialization;

namespace Persistence.Handlers.Specialization
{
    public class SpecializationQueryHandler(HmsDbContext context) :
        IRequestHandler<FindSpecsByName, IEnumerable<SpecializationViewModel>>
    {
        public async Task<IEnumerable<SpecializationViewModel>> Handle(FindSpecsByName request, CancellationToken cancellationToken)
        {
            return await context.Specializations
                .Where(s => s.Name.Contains(request.Name))
                .ProjectToType<SpecializationViewModel>()
                .ToListAsync(cancellationToken);
        }
    }
}
