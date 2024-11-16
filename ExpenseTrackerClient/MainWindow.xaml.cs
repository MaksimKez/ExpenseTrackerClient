using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;
using ExpenseTrackerClient.Models;
using Newtonsoft.Json;

namespace ExpenseTrackerClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    private List<Income> _incomes;
    private List<Expense> _expenses;
    private TransactionsClient _httpClient;
    private const string FILE_PATH = "C:\\Users\\prost\\Desktop\\ExpenseTrackerClient\\ExpenseTrackerClient\\UserAndAccountData.json";

    public MainWindow()
    {
        InitializeComponent();
        _incomes = new List<Income>();
        _expenses = new List<Expense>();
        _httpClient = new TransactionsClient();
    }

    // Загружаем данные после инициализации UI
    private async void Window_Loaded(object sender, RoutedEventArgs routedEventArgs)
    {
        try
        {
            var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
            _incomes = await _httpClient.GetIncomesByBankAccountIdAsync(bankAccountId);
            _expenses = await _httpClient.GetExpensesByBankAccountIdAsync(bankAccountId);
            DataContext = this;
            
            IncomesListBox.ItemsSource = _incomes;
            ExpensesListBox.ItemsSource = _expenses;

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
        }
    }
    
    private Guid GetBankAccountIdFromJson(string filePath)
    {
        /*if (!File.Exists(filePath))
        {
            throw new ArgumentException("Файл с данными пользователя не найден.", filePath);
        }*/

        var jsonData = File.ReadAllText(filePath);

        // Десериализуем JSON данные
        var userAndAccountData = JsonConvert.DeserializeObject<UserAndAccountData>(jsonData);

        if (userAndAccountData == null || userAndAccountData.BankAccountId == Guid.Empty)
        {
            throw new ArgumentException("Неверный формат файла JSON или отсутствует BankAccountId.");
        }

        return userAndAccountData.BankAccountId;
    }


    public async Task AddIncomeAsync(Income newIncome)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        await _httpClient.AddIncomeAsync(bankAccountId, newIncome);
    
        _incomes.Add(newIncome);  // Обновление коллекции
    }

    public async Task AddExpenseAsync(Expense newExpense)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        await _httpClient.AddExpenseAsync(bankAccountId, newExpense);
    
        _expenses.Add(newExpense);  // Обновление коллекции
    }

    public async Task RemoveIncomeAsync(Guid incomeId)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        await _httpClient.DeleteIncomeAsync(bankAccountId, incomeId);
    
        var incomeToRemove = _incomes.FirstOrDefault(i => i.Id == incomeId);
        if (incomeToRemove != null)
        {
            _incomes.Remove(incomeToRemove);  // Обновление коллекции
        }
    }

    public async Task RemoveExpenseAsync(Guid expenseId)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        await _httpClient.DeleteExpenseAsync(bankAccountId, expenseId);
    
        var expenseToRemove = _expenses.FirstOrDefault(e => e.Id == expenseId);
        if (expenseToRemove != null)
        {
            _expenses.Remove(expenseToRemove);  // Обновление коллекции
        }
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private void LogOutButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}