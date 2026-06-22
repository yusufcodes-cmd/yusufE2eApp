using FinanceTracker.API.DTOs;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryRepository.GetAllAsync();

        var result = categories.Select(c => new CategoryDto(
            c.Id, c.Name, c.Icon, c.Colour, c.IsDefault
        ));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
            return NotFound();

        return Ok(new CategoryDto(
            category.Id, category.Name, category.Icon, category.Colour, category.IsDefault
        ));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Icon = dto.Icon,
            Colour = dto.Colour,
            IsDefault = false
        };

        await _categoryRepository.AddAsync(category);

        var result = new CategoryDto(
            category.Id, category.Name, category.Icon, category.Colour, category.IsDefault
        );

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
            return NotFound();

        await _categoryRepository.DeleteAsync(id);

        return NoContent();
    }
}
