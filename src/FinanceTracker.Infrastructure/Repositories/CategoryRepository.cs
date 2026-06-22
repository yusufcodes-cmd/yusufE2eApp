using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetDefaultCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsDefault)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
