using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Queries.Shared;
using System.Linq.Expressions;
using X.PagedList;
using X.PagedList.EF;

namespace Persistence.Handlers.Base
{
    public class PagedQueryHandlerBase<TEntity, TDataModel>(HmsDbContext context, Expression<Func<TEntity, string>> keySelector) :
        IRequestHandler<GetPagedModels<TDataModel>, IPagedList<TDataModel>>
        where TEntity : class
        where TDataModel : class
    {
        protected HmsDbContext Context { get; } = context;

        public virtual async Task<IPagedList<TDataModel>> Handle(GetPagedModels<TDataModel> request, CancellationToken cancellationToken)
        {
            var totalCount = await Context.Set<TEntity>().CountAsync(cancellationToken);
            return await Context.Set<TEntity>()
                .OrderBy(keySelector)
                .ProjectToType<TDataModel>()
                .ToPagedListAsync(request.PageNumber, request.PageSize, totalCount);
        }
    }
}
