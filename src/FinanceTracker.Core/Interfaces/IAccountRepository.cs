using FinanceTracker.Core.Entities;

namespace FinanceTracker.Core.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Task<IReadOnlyList<Account>> GetActiveAccountsByUserAsync(Guid userId);
    Task<decimal> GetTotalBalanceByUserAsync(Guid userId);
}
