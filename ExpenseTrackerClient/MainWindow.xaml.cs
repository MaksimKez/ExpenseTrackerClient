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
    private ObservableCollection<Income> _incomes;
    private ObservableCollection<Expense> _expenses;
    private TransactionsClient _httpClient;
    private const string FILE_PATH = "C:\\Users\\prost\\Desktop\\ExpenseTrackerClient\\ExpenseTrackerClient\\UserAndAccountData.json";

    public MainWindow()
    {
        InitializeComponent();
        _incomes = new ObservableCollection<Income>();
        _expenses = new ObservableCollection<Expense>();
        _httpClient = new TransactionsClient();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs routedEventArgs)
    {
        try
        {
            var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);
            
            // Загрузка доходов и расходов
            await LoadDataAsync(bankAccountId);
            
            // Привязка источников данных
            IncomesListBox.ItemsSource = _incomes;
            ExpensesListBox.ItemsSource = _expenses;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private Guid GetBankAccountIdFromJson(string filePath)
    {
        var jsonData = File.ReadAllText(filePath);

        var accountData = JsonConvert.DeserializeObject<AccountData>(jsonData);
        if (accountData == null || accountData.BankAccountId == Guid.Empty)
        {
            throw new ArgumentException("Неверный формат файла JSON или отсутствует BankAccountId.");
        }

        return accountData.BankAccountId;
    }

    private async Task LoadDataAsync(Guid bankAccountId)
    {
        _incomes.Clear();
        var incomesFromApi = await _httpClient.GetIncomesByBankAccountIdAsync(bankAccountId);
        foreach (var income in incomesFromApi)
        {
            _incomes.Add(income);
        }

        _expenses.Clear();
        var expensesFromApi = await _httpClient.GetExpensesByBankAccountIdAsync(bankAccountId);
        foreach (var expense in expensesFromApi)
        {
            _expenses.Add(expense);
        }
    }

    public async void AddIncomeAsync(object sender, RoutedEventArgs routedEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);

        var addIncomeWindow = new AddIncomeWindow(bankAccountId, _httpClient);
        addIncomeWindow.ShowDialog(); // Дождаться закрытия окна

        await LoadDataAsync(bankAccountId);
    }

    public async void AddExpenseAsync(object sender, RoutedEventArgs routedEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);

        var addExpenseWindow = new AddExpenseWindow(bankAccountId, _httpClient);
        addExpenseWindow.ShowDialog(); // Дождаться закрытия окна

        await LoadDataAsync(bankAccountId);
    }

    public async void RemoveIncomeAsync(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);

        if (IncomesListBox.SelectedItem is Income selectedIncome)
        {
            var deleteIncomeWindow = new DeleteIncomeWindow(selectedIncome.Id, bankAccountId, _httpClient);
            deleteIncomeWindow.ShowDialog();

            await LoadDataAsync(bankAccountId);
        }
        else
        {
            MessageBox.Show("Выберите доход для удаления.", "Удаление дохода", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    public async void RemoveExpenseAsync(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        var bankAccountId = GetBankAccountIdFromJson(FILE_PATH);

        if (ExpensesListBox.SelectedItem is Expense selectedExpense)
        {
            var deleteExpenseWindow = new DeleteExpenseWindow(selectedExpense.Id, bankAccountId, _httpClient);
            deleteExpenseWindow.ShowDialog();

            await LoadDataAsync(bankAccountId);
        }
        else
        {
            MessageBox.Show("Выберите расход для удаления.", "Удаление расхода", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void LogOutButton_Click(object sender, RoutedEventArgs e)
    {
        var registerOrLogInWindow = new RegisterOrLogInWindow();
        registerOrLogInWindow.Show();
        Close();
    }
}
