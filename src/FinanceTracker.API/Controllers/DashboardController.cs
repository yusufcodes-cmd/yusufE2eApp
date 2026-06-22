using FinanceTracker.API.DTOs;
using FinanceTracker.Core.Enums;
using FinanceTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public DashboardController(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var now = DateTime.UtcNow;

        var totalBalance = await _accountRepository.GetTotalBalanceAsync();
        var accounts = await _accountRepository.GetActiveAccountsAsync();
        var monthlyIncome = await _transactionRepository.GetTotalByTypeAsync(
            TransactionType.Income, now.Month, now.Year);
        var monthlyExpenses = await _transactionRepository.GetTotalByTypeAsync(
            TransactionType.Expense, now.Month, now.Year);
        var transactions = await _transactionRepository.GetByDateRangeAsync(
            new DateTime(now.Year, now.Month, 1), now);

        return Ok(new DashboardDto(
            totalBalance,
            monthlyIncome,
            monthlyExpenses,
            accounts.Count,
            transactions.Count
        ));
    }
}
