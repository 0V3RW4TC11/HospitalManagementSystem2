using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Queries.Shared;
using System.Linq.Expressions;
using X.PagedList;
using X.PagedList.EF;

namespace Persistence.Handlers.Base
{
    public class PagedModelsQueryHandler<TEntity, TResult>(HmsDbContext context, Expression<Func<TEntity, string>> keySelector) :
        IRequestHandler<GetPagedModels<TResult>, IPagedList<TResult>>
        where TEntity : class
        where TResult : class
    {
        public async Task<IPagedList<TResult>> Handle(GetPagedModels<TResult> request, CancellationToken cancellationToken)
        {
            var totalCount = await context.Set<TEntity>().CountAsync(cancellationToken);
            return await context.Set<TEntity>()
                .OrderBy(keySelector)
                .ProjectToType<TResult>()
                .ToPagedListAsync(request.PageNumber, request.PageSize, totalCount);
        }
    }
}
