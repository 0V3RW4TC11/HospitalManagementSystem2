using Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly RepositoryDbContext _context;

    public UnitOfWork(RepositoryDbContext context) => _context = context;

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}