using FinanceTracker.Core.Enums;

namespace FinanceTracker.Core.Entities;

public class Transaction : BaseEntity
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
