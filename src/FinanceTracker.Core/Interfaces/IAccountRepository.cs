using FinanceTracker.Core.Entities;

namespace FinanceTracker.Core.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Task<IReadOnlyList<Account>> GetActiveAccountsAsync();
    Task<decimal> GetTotalBalanceAsync();
}
