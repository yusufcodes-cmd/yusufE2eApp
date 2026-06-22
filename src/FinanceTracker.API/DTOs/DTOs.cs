namespace FinanceTracker.API.DTOs;

// --- Auth DTOs ---

public record RegisterDto(
    string Email,
    string Password,
    string FirstName,
    string LastName
);

public record LoginDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Token
);

// --- Account DTOs ---

public record AccountDto(
    Guid Id,
    string Name,
    string Type,
    decimal Balance,
    string Currency,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateAccountDto(
    string Name,
    string Type,
    decimal Balance,
    string Currency
);

public record UpdateAccountDto(
    string Name,
    string Type,
    decimal Balance,
    string Currency,
    bool IsActive
);

// --- Transaction DTOs ---

public record TransactionDto(
    Guid Id,
    decimal Amount,
    string Description,
    string Type,
    DateTime Date,
    string? Notes,
    Guid AccountId,
    string AccountName,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt
);

public record CreateTransactionDto(
    decimal Amount,
    string Description,
    string Type,
    DateTime Date,
    string? Notes,
    Guid AccountId,
    Guid CategoryId
);

// --- Category DTOs ---

public record CategoryDto(
    Guid Id,
    string Name,
    string Icon,
    string Colour,
    bool IsDefault
);

public record CreateCategoryDto(
    string Name,
    string Icon,
    string Colour
);

// --- Budget DTOs ---

public record BudgetDto(
    Guid Id,
    decimal Amount,
    int Month,
    int Year,
    Guid CategoryId,
    string CategoryName
);

public record CreateBudgetDto(
    decimal Amount,
    int Month,
    int Year,
    Guid CategoryId
);

// --- Dashboard DTO ---

public record DashboardDto(
    decimal TotalBalance,
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    int TotalAccounts,
    int TotalTransactions
);
