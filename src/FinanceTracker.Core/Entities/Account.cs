using FinanceTracker.Core.Enums;

namespace FinanceTracker.Core.Entities;

public class Account : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "GBP";
    public bool IsActive { get; set; } = true;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
