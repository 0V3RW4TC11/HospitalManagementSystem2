using Domain;

namespace Services.Helpers;

public static class TransactionHelper
{
    public static async Task ExecuteInTransactionAsync(IUnitOfWork unitOfWork, Func<Task> action)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();
            await action();
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}