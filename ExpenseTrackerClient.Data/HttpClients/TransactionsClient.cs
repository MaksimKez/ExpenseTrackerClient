using ExpenseTrackerClient.Data.HttpClients.Contracts;

namespace ExpenseTrackerClient.Data.HttpClients;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Models;

public class TransactionsClient : ITransactionClient
{   
    private const string CONNECTION_STRING = "https://localhost:44388/";
    private readonly HttpClient _httpClient;
    
    // YES Ik it is also very bad :)
    public TransactionsClient()
    {
        _httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }

        })
        {
            
            BaseAddress = new Uri(CONNECTION_STRING)
        };
    }

    #region Get income and expenses by bank account id

    public async Task<List<Income>> GetIncomesByBankAccountIdAsync(Guid bankAccountId)
    {
        var incomes = await _httpClient.GetFromJsonAsync<List<Income>>($"bankaccounts/{bankAccountId}/incomes");
        return incomes ?? [];
    }

    public async Task<List<Expense>> GetExpensesByBankAccountIdAsync(Guid bankAccountId)
    {
        var expenses = await _httpClient.GetFromJsonAsync<List<Expense>>($"bankaccounts/{bankAccountId}/expenses");
        return expenses ?? [];
    }

    #endregion

    #region delete income and expense

    public async Task<bool> DeleteIncomeAsync(Guid id, Guid bankAccountId)
    {
        var response = await _httpClient.DeleteAsync($"delete/incomes/{id}/{bankAccountId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteExpenseAsync(Guid id, Guid bankAccountId)
    {
        var response = await _httpClient.DeleteAsync($"delete/expenses/{id}/{bankAccountId}");
        return response.IsSuccessStatusCode;
    }

    #endregion

    #region add income and expense

    public async Task<Guid> AddIncomeAsync(Guid bankAccountId, Income income)
    {
        var response = await _httpClient.PostAsJsonAsync($"add/incomes/{bankAccountId}", income);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Guid>() : Guid.Empty;
    }

    public async Task<Guid> AddExpenseAsync(Guid bankAccountId, Expense expense)
    {
        var response = await _httpClient.PostAsJsonAsync($"add/expenses/{bankAccountId}", expense);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Guid>() : Guid.Empty;
    }

    #endregion

    #region get income and expense by id

    public async Task<Expense?> GetExpenseByIdAsync(Guid id)
    {
        var expense = await _httpClient.GetFromJsonAsync<Expense>($"get/expenses/{id}");
        return expense != null && expense.IsValid() ? expense : null;
    }

    public async Task<Income?> GetIncomeByIdAsync(Guid id)
    {
        var income = await _httpClient.GetFromJsonAsync<Income>($"get/incomes/{id}");
        return income != null && income.IsValid() ? income : null; 
    }

    #endregion

    #region CRUD on BankAccount

    public async Task<Guid> CreateBankAccountAsync(BankAccount bankAccount)
    {
        var response = await _httpClient.PostAsJsonAsync("create/bankaccount", bankAccount);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Guid>() : Guid.Empty;
    }

    public async Task<BankAccount> GetBankAccountByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<BankAccount>($"get/bankaccount/{id}");
    }

    public async Task<bool> DeleteBankAccountAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"delete/bankaccount/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateBankAccountAsync(BankAccount bankAccount)
    {
        var response = await _httpClient.PutAsJsonAsync("update/bankaccount", bankAccount);
        return response.IsSuccessStatusCode;
    }

    #endregion
}
