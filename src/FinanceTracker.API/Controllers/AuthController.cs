using FinanceTracker.API.DTOs;
using FinanceTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        try
        {
            var (user, token) = await _authService.RegisterAsync(
                dto.Email, dto.Password, dto.FirstName, dto.LastName);

            return Ok(new AuthResponseDto(
                user.Id, user.Email, user.FirstName, user.LastName, token));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto.Email, dto.Password);

        if (result is null)
            return Unauthorized(new { message = "Invalid email or password." });

        var (user, token) = result.Value;

        return Ok(new AuthResponseDto(
            user.Id, user.Email, user.FirstName, user.LastName, token));
    }
}
