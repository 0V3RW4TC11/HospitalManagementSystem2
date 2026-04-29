using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Queries.Shared;
using System.Linq.Expressions;
using X.PagedList;
using X.PagedList.EF;

namespace Persistence.Handlers.Base
{
    public class PagedQueryHandlerBase<TEntity, TIndexModel>(HmsDbContext context, Expression<Func<TEntity, string>> keySelector) :
        IRequestHandler<GetPagedModels<TIndexModel>, IPagedList<TIndexModel>>
        where TEntity : class
        where TIndexModel : class
    {
        public async Task<IPagedList<TIndexModel>> Handle(GetPagedModels<TIndexModel> request, CancellationToken cancellationToken)
        {
            var totalCount = await context.Set<TEntity>().CountAsync(cancellationToken);
            return await context.Set<TEntity>()
                .OrderBy(keySelector)
                .ProjectToType<TIndexModel>()
                .ToPagedListAsync(request.PageNumber, request.PageSize, totalCount);
        }
    }
}
