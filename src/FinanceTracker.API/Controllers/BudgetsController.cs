using FinanceTracker.API.DTOs;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetRepository _budgetRepository;

    public BudgetsController(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetByMonth(
        [FromQuery] int month, [FromQuery] int year)
    {
        var budgets = await _budgetRepository.GetByMonthAsync(month, year);

        var result = budgets.Select(b => new BudgetDto(
            b.Id, b.Amount, b.Month, b.Year, b.CategoryId, b.Category?.Name ?? ""
        ));

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto>> Create(CreateBudgetDto dto)
    {
        var existing = await _budgetRepository.GetByCategoryAndMonthAsync(
            dto.CategoryId, dto.Month, dto.Year);

        if (existing is not null)
            return Conflict("A budget already exists for this category and month.");

        var budget = new Budget
        {
            Amount = dto.Amount,
            Month = dto.Month,
            Year = dto.Year,
            CategoryId = dto.CategoryId
        };

        await _budgetRepository.AddAsync(budget);

        var result = new BudgetDto(
            budget.Id, budget.Amount, budget.Month, budget.Year, budget.CategoryId, ""
        );

        return CreatedAtAction(nameof(GetByMonth), new { month = budget.Month, year = budget.Year }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);

        if (budget is null)
            return NotFound();

        await _budgetRepository.DeleteAsync(id);

        return NoContent();
    }
}
