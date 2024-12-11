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
    private const string FILE_PATH = "C:\\Users\\Tonya\\Desktop\\kursClient\\ExpenseTrackerClient\\UserAndAccountData.json";

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
        this.Close();
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        SearchFilterWindow filterWindow = new SearchFilterWindow();
        filterWindow.ShowDialog();
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        SortCriteriaWindow sortWindow = new SortCriteriaWindow();
        if (sortWindow.ShowDialog() == true)
        {
            SortRecords(sortWindow.SelectedCriteria);
        }
    }

    private void ReportButton_Click(object sender, RoutedEventArgs e)
    {
        var reportWindow = new ReportWindow();
        reportWindow.Show();
        this.Close();
    }

    private void SortRecords(string criteria) 
    { 
        switch (criteria) 
        { 
            case "Date": 
                _incomes = new ObservableCollection<Income>(_incomes.OrderBy<Income, object>(x => x.Date)); // Встроенная сортировка
                _expenses = new ObservableCollection<Expense>(_expenses.OrderBy(x => x.Date)); // Встроенная сортировка
                break; 
            case "Category":
                BubbleSort(_incomes); // Пузырьковая сортировка
                BubbleSort(_expenses); // Пузырьковая сортировка
                break;
            case "Amount": 
                ShakerSort(_incomes); // Шейкерная сортировка
                ShakerSort(_expenses); // Шейкерная сортировка
                break; 
        } 
        DataContext = null; 
        DataContext = this; 
    }
    
    public static void BubbleSort(IncomeSourceEnum[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - 1 - i; j++)
            {
                if (array[j] > array[j + 1])
                {
                    IncomeSourceEnum temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }
    
    public static void BubbleSort(ExpenseSourceEnum[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - 1 - i; j++)
            {
                if (array[j] > array[j + 1])
                {
                    ExpenseSourceEnum temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }

    private void ShakerSort(ObservableCollection<Income> records)
    {
        int left = 0; int right = records.Count - 1;
        while (left < right)
        {
            for (int i = left; i < right; i++)
            {
                if (records[i].Sum > records[i + 1].Sum)
                {
                    var temp = records[i]; records[i] = records[i + 1]; records[i + 1] = temp;
                }
            } 
            right--;
            for (int i = right; i > left; i--)
            {
                if (records[i - 1].Sum > records[i].Sum)
                {
                    var temp = records[i - 1]; records[i - 1] = records[i]; records[i] = temp;
                }
            } 
            left++;
        }
    }

    private void ShakerSort(ObservableCollection<Expense> records)
    {
        int left = 0;
        int right = records.Count - 1;
        while (left < right)
        {
            for (int i = left; i < right; i++)
            {
                if (records[i].Sum > records[i + 1].Sum)
                {
                    var temp = records[i];
                    records[i] = records[i + 1];
                    records[i + 1] = temp;
                }
            }

            right--;
            for (int i = right; i > left; i--)
            {
                if (records[i - 1].Sum > records[i].Sum)
                {
                    var temp = records[i - 1];
                    records[i - 1] = records[i];
                    records[i] = temp;
                }
            }

            left++;
        }
    }
}