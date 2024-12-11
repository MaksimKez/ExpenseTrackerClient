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
                _incomes = new ObservableCollection<Income>(_incomes.OrderBy(x => x.CreatedAt)); // Встроенная сортировка
                _expenses = new ObservableCollection<Expense>(_expenses.OrderBy(x => x.CreatedAt)); // Встроенная сортировка
                break; 
            case "Category": 
                _incomes = new ObservableCollection<Income>(BubbleSort(_incomes.ToList())); // Пузырьковая сортировка
                _expenses = new ObservableCollection<Expense>(BubbleSort(_expenses.ToList())); // Пузырьковая сортировка
                break;
            case "Amount": 
                _incomes = new ObservableCollection<Income>(ShakerSort(_incomes.ToList())); // Шейкерная сортировка
                _expenses = new ObservableCollection<Expense>(ShakerSort(_expenses.ToList())); // Шейкерная сортировка
                break;
        } 
        DataContext = null; 
        DataContext = this; 
    }
    
    public static List<Income> BubbleSort(List<Income> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - 1 - i; j++)
            {
                if (list[j].IncomeSource > list[j + 1].IncomeSource)
                {
                    var temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
        return list;
    }

    public static List<Expense> BubbleSort(List<Expense> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - 1 - i; j++)
            {
                if (list[j].ExpenseSource > list[j + 1].ExpenseSource)
                {
                    var temp = list[j]; list[j] = list[j + 1]; list[j + 1] = temp;
                }
            }
        } 
        return list;
    }

    public static List<Income> ShakerSort(List<Income> list)
    {
        int left = 0; 
        int right = list.Count - 1;
        while (left <= right)
        {
            for (int i = left; i < right; i++)
            {
                if (list[i].Sum > list[i + 1].Sum)
                {
                    var temp = list[i]; 
                    list[i] = list[i + 1];
                    list[i + 1] = temp;
                }
            } 
            right--;
            for (int i = right; i > left; i--)
            {
                if (list[i - 1].Sum > list[i].Sum)
                {
                    var temp = list[i]; 
                    list[i] = list[i - 1]; 
                    list[i - 1] = temp;
                }
            } 
            left++;
        } 
        return list;
    }

    public static List<Expense> ShakerSort(List<Expense> list)
    {
        int left = 0; 
        int right = list.Count - 1;
        while (left <= right)
        {
            for (int i = left; i < right; i++)
            {
                if (list[i].Sum > list[i + 1].Sum)
                {
                    var temp = list[i]; 
                    list[i] = list[i + 1]; 
                    list[i + 1] = temp;
                }
            } 
            right--;
            for (int i = right; i > left; i--)
            {
                if (list[i - 1].Sum > list[i].Sum)
                {
                    var temp = list[i];
                    list[i] = list[i - 1]; 
                    list[i - 1] = temp;
                }
            }
            left++;
        } 
        return list;
    } 
}