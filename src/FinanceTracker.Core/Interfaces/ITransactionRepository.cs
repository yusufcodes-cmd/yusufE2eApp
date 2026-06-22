using FinanceTracker.Core.Entities;

namespace FinanceTracker.Core.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId);
    Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<Transaction>> GetByCategoryIdAsync(Guid categoryId);
    Task<decimal> GetTotalByTypeAsync(Enums.TransactionType type, int month, int year);
}
