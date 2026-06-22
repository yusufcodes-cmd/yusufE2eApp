using FinanceTracker.Core.Entities;

namespace FinanceTracker.Core.Interfaces;

public interface IBudgetRepository : IRepository<Budget>
{
    Task<IReadOnlyList<Budget>> GetByMonthAsync(Guid userId, int month, int year);
    Task<Budget?> GetByCategoryAndMonthAsync(Guid userId, Guid categoryId, int month, int year);
}
