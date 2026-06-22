using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Enums;
using FinanceTracker.Core.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Transaction>> GetAllByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.Account.UserId == userId)
            .Include(t => t.Category)
            .Include(t => t.Account)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId)
    {
        return await _dbSet
            .Where(t => t.AccountId == accountId)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(t => t.Account.UserId == userId && t.Date >= startDate && t.Date <= endDate)
            .Include(t => t.Category)
            .Include(t => t.Account)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(t => t.CategoryId == categoryId)
            .Include(t => t.Account)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalByTypeAsync(Guid userId, TransactionType type, int month, int year)
    {
        return await _dbSet
            .Where(t => t.Account.UserId == userId && t.Type == type && t.Date.Month == month && t.Date.Year == year)
            .SumAsync(t => t.Amount);
    }
}
