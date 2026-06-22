using System.Security.Claims;
using FinanceTracker.API.DTOs;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Enums;
using FinanceTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;

    public TransactionsController(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
    {
        var transactions = await _transactionRepository.GetAllByUserAsync(GetUserId());

        var result = transactions.Select(t => new TransactionDto(
            t.Id, t.Amount, t.Description, t.Type.ToString(), t.Date, t.Notes,
            t.AccountId, t.Account?.Name ?? "", t.CategoryId, t.Category?.Name ?? "", t.CreatedAt
        ));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> GetById(Guid id)
    {
        var t = await _transactionRepository.GetByIdAsync(id);

        if (t is null || t.Account?.UserId != GetUserId())
            return NotFound();

        return Ok(new TransactionDto(
            t.Id, t.Amount, t.Description, t.Type.ToString(), t.Date, t.Notes,
            t.AccountId, t.Account?.Name ?? "", t.CategoryId, t.Category?.Name ?? "", t.CreatedAt
        ));
    }

    [HttpGet("by-account/{accountId:guid}")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByAccount(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null || account.UserId != GetUserId())
            return NotFound();

        var transactions = await _transactionRepository.GetByAccountIdAsync(accountId);

        var result = transactions.Select(t => new TransactionDto(
            t.Id, t.Amount, t.Description, t.Type.ToString(), t.Date, t.Notes,
            t.AccountId, t.Account?.Name ?? "", t.CategoryId, t.Category?.Name ?? "", t.CreatedAt
        ));

        return Ok(result);
    }

    [HttpGet("by-date-range")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByDateRange(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var transactions = await _transactionRepository.GetByDateRangeAsync(GetUserId(), startDate, endDate);

        var result = transactions.Select(t => new TransactionDto(
            t.Id, t.Amount, t.Description, t.Type.ToString(), t.Date, t.Notes,
            t.AccountId, t.Account?.Name ?? "", t.CategoryId, t.Category?.Name ?? "", t.CreatedAt
        ));

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionDto dto)
    {
        var account = await _accountRepository.GetByIdAsync(dto.AccountId);
        if (account is null || account.UserId != GetUserId())
            return BadRequest("Invalid account.");

        var transaction = new Transaction
        {
            Amount = dto.Amount,
            Description = dto.Description,
            Type = Enum.Parse<TransactionType>(dto.Type),
            Date = dto.Date,
            Notes = dto.Notes,
            AccountId = dto.AccountId,
            CategoryId = dto.CategoryId
        };

        await _transactionRepository.AddAsync(transaction);

        // Update account balance
        account.Balance += transaction.Type == TransactionType.Income
            ? transaction.Amount
            : -transaction.Amount;
        await _accountRepository.UpdateAsync(account);

        var result = new TransactionDto(
            transaction.Id, transaction.Amount, transaction.Description,
            transaction.Type.ToString(), transaction.Date, transaction.Notes,
            transaction.AccountId, account.Name, transaction.CategoryId, "", transaction.CreatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);

        if (transaction is null)
            return NotFound();

        var account = await _accountRepository.GetByIdAsync(transaction.AccountId);
        if (account is null || account.UserId != GetUserId())
            return NotFound();

        await _transactionRepository.DeleteAsync(id);

        return NoContent();
    }
}
