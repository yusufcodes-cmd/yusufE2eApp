using FinanceTracker.Core.Entities;

namespace FinanceTracker.Core.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId);
    Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<Transaction>> GetByCategoryIdAsync(Guid categoryId);
    Task<IReadOnlyList<Transaction>> GetAllByUserAsync(Guid userId);
    Task<decimal> GetTotalByTypeAsync(Guid userId, Enums.TransactionType type, int month, int year);
}
