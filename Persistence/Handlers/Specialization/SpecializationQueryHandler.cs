using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Handlers.Base;
using Queries.Specialization;
using ViewModels.Specialization;

namespace Persistence.Handlers.Specialization
{
    public class SpecializationQueryHandler(HmsDbContext context) :
        PagedQueryHandlerBase<Entities.Specialization, SpecViewModel>(context, x => x.Name),
        IRequestHandler<GetEditSpecModel, SpecViewModel>,
        IRequestHandler<FindSpecsByName, IEnumerable<SpecViewModel>>
    {
        public async Task<IEnumerable<SpecViewModel>> Handle(FindSpecsByName request, CancellationToken cancellationToken)
        {
            return await Context.Specializations
                .Where(s => s.Name.Contains(request.Name))
                .ProjectToType<SpecViewModel>()
                .ToListAsync(cancellationToken);
        }

        public async Task<SpecViewModel> Handle(GetEditSpecModel request, CancellationToken cancellationToken)
        {
            var entity = await Context.Specializations.SingleAsync(s => s.Id == request.Id, cancellationToken);
            return entity.Adapt<SpecViewModel>();
        }
    }
}
