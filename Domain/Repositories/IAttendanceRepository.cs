using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories;

public interface IAttendanceRepository
{
    void Add(Attendance attendance);

    Task<bool> ExistsAsync(Expression<Func<Attendance, bool>> predicate);

    Task<Attendance?> FindByIdAsync(Guid id);

    Task<(TResult[] List, int TotalCount)> GetPagedListAsync<TResult>(
        Expression<Func<Attendance, bool>> predicate, 
        Expression<Func<Attendance, TResult>> selector,
        int pageNumber,
        int pageSize);

    void Remove(Attendance attendance);
}