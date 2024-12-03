using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            //  отобразить окно регистрации/входа
        }
    }
    
    private Guid GetBankAccountIdFromJson(string filePath)
    {
        var jsonData = File.ReadAllText(filePath);

        // Десериализуем JSON данные
        var accountData = JsonConvert.DeserializeObject<AccountData>(jsonData);

        if (accountData == null && accountData.BankAccountId == Guid.Empty)
        {
            throw new ArgumentException("Неверный формат файла JSON или отсутствует BankAccountId.");
        }

        return accountData.BankAccountId;
    }


    public async void AddIncomeAsync(object sender, RoutedEventArgs routedEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        
        var AddIncomeWindow = new AddIncomeWindow(bankAccountId, _httpClient);
    
        AddIncomeWindow.Show();
         
        _incomes = await _httpClient.GetIncomesByBankAccountIdAsync(bankAccountId);
        IncomesListBox.ItemsSource = _incomes;  // Обновление коллекции
    }

    public async void AddExpenseAsync(object sender, RoutedEventArgs routedEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        
        var addExpenseWindow = new AddExpenseWindow(bankAccountId, _httpClient);
    
        addExpenseWindow.Show();
        
        _expenses = await _httpClient.GetExpensesByBankAccountIdAsync(bankAccountId);

        _incomes = await _httpClient.GetIncomesByBankAccountIdAsync(bankAccountId);
        ExpensesListBox.ItemsSource = _expenses; // Обновление коллекции
    }

    public async void RemoveIncomeAsync(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        
        var incomeId = (IncomesListBox.SelectedItem as Income).Id;
        if (incomeId.Equals(Guid.Empty))
            throw new ArgumentException("Выберите доход.");
        
        var deleteIncomeWindow = new DeleteIncomeWindow(incomeId, bankAccountId, _httpClient);
        deleteIncomeWindow.Show();
        
        IncomesListBox.ItemsSource = _incomes;
    }

    public async void RemoveExpenseAsync(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
        
        var expenseId = (ExpensesListBox.SelectedItem as Expense).Id;
        if (expenseId.Equals(Guid.Empty))
            throw new ArgumentException("Выберите расход.");
        
        var deleteExpenseWindow = new DeleteExpenseWindow(expenseId, bankAccountId, _httpClient);
        deleteExpenseWindow.Show();
        
        ExpensesListBox.ItemsSource = _expenses;
    }

    
    private void LogOutButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            File.WriteAllText(FILE_PATH, string.Empty);
            MessageBox.Show("Файл очищен успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        catch
        {
            MessageBox.Show($"Ошибка при выходе из аккаунта:", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow();
        registerWindow.Show();
    }
    
    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        IncomesListBox.ItemsSource = _incomes;
        ExpensesListBox.ItemsSource = _expenses;
    }
}