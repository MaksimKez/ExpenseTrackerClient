using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient.Data.HttpClients.Contracts;

public interface ITransactionClient
{
    Task<List<Income>> GetIncomesByBankAccountIdAsync(Guid bankAccountId);
    Task<List<Expense>> GetExpensesByBankAccountIdAsync(Guid bankAccountId);
    Task<bool> DeleteIncomeAsync(Guid id, Guid bankAccountId);
    Task<bool> DeleteExpenseAsync(Guid id, Guid bankAccountId);
    Task<Guid> AddIncomeAsync(Guid bankAccountId, Income income);
    Task<Guid> AddExpenseAsync(Guid bankAccountId, Expense expense);
    Task<Expense?> GetExpenseByIdAsync(Guid id);
    Task<Income?> GetIncomeByIdAsync(Guid id);
    Task<Guid> CreateBankAccountAsync(BankAccount bankAccount);
    Task<BankAccount> GetBankAccountByIdAsync(Guid id);
    Task<bool> DeleteBankAccountAsync(Guid id);
    Task<bool> UpdateBankAccountAsync(BankAccount bankAccount);
}