using System.Security.Claims;
using FinanceTracker.API.DTOs;
using FinanceTracker.Core.Enums;
using FinanceTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[Authorize]
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

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var userId = GetUserId();
        var now = DateTime.UtcNow;

        var totalBalance = await _accountRepository.GetTotalBalanceByUserAsync(userId);
        var accounts = await _accountRepository.GetActiveAccountsByUserAsync(userId);
        var monthlyIncome = await _transactionRepository.GetTotalByTypeAsync(
            userId, TransactionType.Income, now.Month, now.Year);
        var monthlyExpenses = await _transactionRepository.GetTotalByTypeAsync(
            userId, TransactionType.Expense, now.Month, now.Year);
        var transactions = await _transactionRepository.GetByDateRangeAsync(
            userId, new DateTime(now.Year, now.Month, 1), now);

        return Ok(new DashboardDto(
            totalBalance,
            monthlyIncome,
            monthlyExpenses,
            accounts.Count,
            transactions.Count
        ));
    }
}
