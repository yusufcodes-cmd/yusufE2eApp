using FinanceTracker.Core.Entities;

namespace FinanceTracker.Core.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IReadOnlyList<Category>> GetDefaultCategoriesAsync();
}
