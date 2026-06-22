using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class BudgetRepository : Repository<Budget>, IBudgetRepository
{
    public BudgetRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Budget>> GetByMonthAsync(Guid userId, int month, int year)
    {
        return await _dbSet
            .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
            .Include(b => b.Category)
            .ToListAsync();
    }

    public async Task<Budget?> GetByCategoryAndMonthAsync(Guid userId, Guid categoryId, int month, int year)
    {
        return await _dbSet
            .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId && b.Month == month && b.Year == year);
    }
}
