using FinanceTracker.Core.Entities;
using FinanceTracker.Infrastructure.Authentication;
using FinanceTracker.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinanceTracker.UnitTests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        var jwtSettings = Options.Create(new JwtSettings
        {
            SecretKey = "ThisIsATestSecretKeyThatIsAtLeast32Characters!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryInHours = 24
        });

        _authService = new AuthService(_context, jwtSettings);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsUserAndToken()
    {
        var (user, token) = await _authService.RegisterAsync(
            "test@example.com", "Password123!", "John", "Doe");

        user.Should().NotBeNull();
        user.Email.Should().Be("test@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegisterAsync_HashesPassword_NotStoredAsPlainText()
    {
        var (user, _) = await _authService.RegisterAsync(
            "test@example.com", "Password123!", "John", "Doe");

        user.PasswordHash.Should().NotBe("Password123!");
        BCrypt.Net.BCrypt.Verify("Password123!", user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsException()
    {
        await _authService.RegisterAsync("test@example.com", "Password123!", "John", "Doe");

        var act = async () => await _authService.RegisterAsync(
            "test@example.com", "AnotherPass1!", "Jane", "Doe");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsUserAndToken()
    {
        await _authService.RegisterAsync("test@example.com", "Password123!", "John", "Doe");

        var result = await _authService.LoginAsync("test@example.com", "Password123!");

        result.Should().NotBeNull();
        result!.Value.User.Email.Should().Be("test@example.com");
        result.Value.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsNull()
    {
        await _authService.RegisterAsync("test@example.com", "Password123!", "John", "Doe");

        var result = await _authService.LoginAsync("test@example.com", "WrongPassword!");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ReturnsNull()
    {
        var result = await _authService.LoginAsync("noone@example.com", "Password123!");

        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_NormalisesEmail_ToLowerCase()
    {
        var (user, _) = await _authService.RegisterAsync(
            "Test@EXAMPLE.com", "Password123!", "John", "Doe");

        user.Email.Should().Be("test@example.com");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
