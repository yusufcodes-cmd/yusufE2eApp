using FinanceTracker.Core.Entities;

namespace FinanceTracker.Core.Interfaces;

public interface IAuthService
{
    Task<(User User, string Token)> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<(User User, string Token)?> LoginAsync(string email, string password);
}
