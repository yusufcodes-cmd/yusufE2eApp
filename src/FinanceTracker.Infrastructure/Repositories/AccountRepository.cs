using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Account>> GetActiveAccountsByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(a => a.UserId == userId && a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalBalanceByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(a => a.UserId == userId && a.IsActive)
            .SumAsync(a => a.Balance);
    }
}
