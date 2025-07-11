using HospitalManagementSystem2.Data;

namespace HospitalManagementSystem2.Helpers;

public class TransactionHelper
{
    public static async Task ExecuteInTransactionAsync(IDbContext context,
        Func<Task> operation)
    {
        await using var transaction = await context.BeginTransactionAsync();
        
        try
        {
            await operation();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}