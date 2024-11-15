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
    
    private ObservableCollection<Income> _incomes;
    private ObservableCollection<Expense> _expenses;
    private TransactionsClient _httpClient;
    private const string FILE_PATH = "userAndAccountData.json";

    public MainWindow()
    {
        InitializeComponent();
    
        _httpClient = new TransactionsClient();

        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);

        _incomes = new ObservableCollection<Income>(_httpClient.GetIncomesByBankAccountIdAsync(bankAccountId).Result);
        _expenses = new ObservableCollection<Expense>(_httpClient.GetExpensesByBankAccountIdAsync(bankAccountId).Result);

        DataContext = this;
    }
    
    public async Task LoadDataAsync()
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);

        var incomes = await _httpClient.GetIncomesByBankAccountIdAsync(bankAccountId);
        _incomes = new ObservableCollection<Income>(incomes);

        var expenses = await _httpClient.GetExpensesByBankAccountIdAsync(bankAccountId);
        _expenses = new ObservableCollection<Expense>(expenses);

        DataContext = this;
    }
    
    private Guid GetBankAccountIdFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл с данными пользователя не найден.", filePath);
        }

        var jsonData = File.ReadAllText(filePath);

        // Десериализуем JSON данные
        var userAndAccountData = JsonConvert.DeserializeObject<UserAndAccountData>(jsonData);

        if (userAndAccountData == null || userAndAccountData.BankAccountId == Guid.Empty)
        {
            throw new InvalidDataException("Неверный формат файла JSON или отсутствует BankAccountId.");
        }

        return userAndAccountData.BankAccountId;
    }


    
    
    
    
    
    
    
    
    
    private void AddIncomeButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void LogOutButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}