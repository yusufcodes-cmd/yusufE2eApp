using FinanceTracker.Core.Entities;

namespace FinanceTracker.Core.Interfaces;

public interface IBudgetRepository : IRepository<Budget>
{
    Task<IReadOnlyList<Budget>> GetByMonthAsync(int month, int year);
    Task<Budget?> GetByCategoryAndMonthAsync(Guid categoryId, int month, int year);
}
