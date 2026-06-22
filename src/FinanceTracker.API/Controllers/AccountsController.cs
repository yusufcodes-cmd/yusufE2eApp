using FinanceTracker.API.DTOs;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Enums;
using FinanceTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;

    public AccountsController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll()
    {
        var accounts = await _accountRepository.GetActiveAccountsAsync();

        var result = accounts.Select(a => new AccountDto(
            a.Id, a.Name, a.Type.ToString(), a.Balance, a.Currency, a.IsActive, a.CreatedAt
        ));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountDto>> GetById(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);

        if (account is null)
            return NotFound();

        return Ok(new AccountDto(
            account.Id, account.Name, account.Type.ToString(),
            account.Balance, account.Currency, account.IsActive, account.CreatedAt
        ));
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountDto dto)
    {
        var account = new Account
        {
            Name = dto.Name,
            Type = Enum.Parse<AccountType>(dto.Type),
            Balance = dto.Balance,
            Currency = dto.Currency
        };

        await _accountRepository.AddAsync(account);

        var result = new AccountDto(
            account.Id, account.Name, account.Type.ToString(),
            account.Balance, account.Currency, account.IsActive, account.CreatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = account.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateAccountDto dto)
    {
        var account = await _accountRepository.GetByIdAsync(id);

        if (account is null)
            return NotFound();

        account.Name = dto.Name;
        account.Type = Enum.Parse<AccountType>(dto.Type);
        account.Balance = dto.Balance;
        account.Currency = dto.Currency;
        account.IsActive = dto.IsActive;

        await _accountRepository.UpdateAsync(account);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);

        if (account is null)
            return NotFound();

        await _accountRepository.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("total-balance")]
    public async Task<ActionResult<decimal>> GetTotalBalance()
    {
        var total = await _accountRepository.GetTotalBalanceAsync();
        return Ok(total);
    }
}
