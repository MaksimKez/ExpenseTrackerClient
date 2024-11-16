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


    public async void AddIncomeAsync(object sender, RoutedEventArgs routedEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        
        var AddIncomeWindow = new AddIncomeWindow(bankAccountId, _httpClient);
    
        AddIncomeWindow.Show();
        
        IncomesListBox.ItemsSource = _incomes;  // Обновление коллекции
    }

    public async void AddExpenseAsync(object sender, RoutedEventArgs routedEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        

        ExpensesListBox.ItemsSource = _expenses; // Обновление коллекции
    }

    public async Task RemoveIncomeAsync(Guid incomeId)
    {
        // todo implement removing
        
        IncomesListBox.ItemsSource = _incomes;
    }

    public async Task RemoveExpenseAsync(Guid expenseId)
    {
        // todo implement removing
        
        ExpensesListBox.ItemsSource = _expenses;
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private void LogOutButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}