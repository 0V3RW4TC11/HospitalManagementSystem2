using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Handlers.Base;
using Queries.Specialization;
using ViewModels.Specialization;

namespace Persistence.Handlers.Specialization
{
    public class SpecializationQueryHandler(HmsDbContext context) :
        PagedQueryHandlerBase<Entities.Specialization, SpecializationViewModel>(context, x => x.Name),
        IRequestHandler<FindSpecsByName, IEnumerable<SpecializationViewModel>>
    {
        private readonly HmsDbContext _context = context;

        public async Task<IEnumerable<SpecializationViewModel>> Handle(FindSpecsByName request, CancellationToken cancellationToken)
        {
            return await _context.Specializations
                .Where(s => s.Name.Contains(request.Name))
                .ProjectToType<SpecializationViewModel>()
                .ToListAsync(cancellationToken);
        }
    }
}
