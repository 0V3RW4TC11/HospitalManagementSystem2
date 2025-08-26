using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Helpers
{
    internal static class PagedListHelper
    {
        public static async Task<IEnumerable<T>> GetPagedList<T>(
            IQueryable<T> list, 
            Expression<Func<T, Guid>> keySelector, 
            int pageNumber, 
            int pageSize)
        {
            // Ensure page number is at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 0 ? 10 : pageSize;

            // Fetch only the rows for the current page
            return await list
                .OrderBy(keySelector)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();
        }
    }
}
