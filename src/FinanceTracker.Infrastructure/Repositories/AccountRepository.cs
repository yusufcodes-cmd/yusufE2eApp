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

    public async Task<IReadOnlyList<Account>> GetActiveAccountsAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalBalanceAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .SumAsync(a => a.Balance);
    }
}
