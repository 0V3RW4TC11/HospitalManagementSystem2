using HospitalManagementSystem2.Data;
using System.Runtime.CompilerServices;

namespace HospitalManagementSystem2.Utility
{
    public class TransactionHelper
    {
        public static async Task ExecuteInTransaction(ApplicationDbContext context, 
                                                      Func<Task> operation)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
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
}
