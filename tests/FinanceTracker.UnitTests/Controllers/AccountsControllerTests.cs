using System.Security.Claims;
using FinanceTracker.API.Controllers;
using FinanceTracker.API.DTOs;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.UnitTests.Controllers;

public class AccountsControllerTests
{
    private readonly Mock<IAccountRepository> _mockRepo;
    private readonly AccountsController _controller;
    private readonly Guid _userId = Guid.NewGuid();

    public AccountsControllerTests()
    {
        _mockRepo = new Mock<IAccountRepository>();
        _controller = new AccountsController(_mockRepo.Object);

        // Simulate authenticated user
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithAccounts()
    {
        var accounts = new List<Account>
        {
            new() { Id = Guid.NewGuid(), Name = "Savings", Balance = 1000, Currency = "GBP", UserId = _userId }
        };
        _mockRepo.Setup(r => r.GetActiveAccountsByUserAsync(_userId))
            .ReturnsAsync(accounts);

        var result = await _controller.GetAll();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var items = (okResult!.Value as IEnumerable<AccountDto>)!.ToList();
        items.Should().HaveCount(1);
        items[0].Name.Should().Be("Savings");
    }

    [Fact]
    public async Task GetById_WhenAccountBelongsToUser_ReturnsOk()
    {
        var accountId = Guid.NewGuid();
        var account = new Account
        {
            Id = accountId, Name = "Current", Balance = 500,
            Currency = "GBP", UserId = _userId
        };
        _mockRepo.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

        var result = await _controller.GetById(accountId);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_WhenAccountBelongsToOtherUser_ReturnsNotFound()
    {
        var accountId = Guid.NewGuid();
        var account = new Account
        {
            Id = accountId, Name = "Other", Balance = 500,
            Currency = "GBP", UserId = Guid.NewGuid() // different user
        };
        _mockRepo.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

        var result = await _controller.GetById(accountId);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_SetsUserIdAndReturnsCreated()
    {
        var dto = new CreateAccountDto("New Account", "Savings", 0, "GBP");

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Account>()))
            .ReturnsAsync((Account a) => a);

        var result = await _controller.Create(dto);

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();

        _mockRepo.Verify(r => r.AddAsync(It.Is<Account>(a => a.UserId == _userId)), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenAccountBelongsToOtherUser_ReturnsNotFound()
    {
        var accountId = Guid.NewGuid();
        var account = new Account
        {
            Id = accountId, UserId = Guid.NewGuid() // different user
        };
        _mockRepo.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

        var result = await _controller.Delete(accountId);

        result.Should().BeOfType<NotFoundResult>();
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
