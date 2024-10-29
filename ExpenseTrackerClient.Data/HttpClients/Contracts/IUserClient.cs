using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient.Data.HttpClients.Contracts;

public interface IUserClient
{
    Task<Guid> RegisterUserAsync(string username, string password, Guid bankAccountId);
    Task<Guid> LoginUserAsync(string username, string password);
    Task<User?> GetUserByIdAsync(Guid id);
}