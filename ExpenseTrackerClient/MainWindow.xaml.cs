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
        SearchCriteriaWindow searchWindow = new SearchCriteriaWindow();
        if (searchWindow.ShowDialog() == true)
        {
            List<object> results = SearchRecords(searchWindow.SelectedCriteria, searchWindow.SearchValue);
            SearchResultsWindow resultsWindow = new SearchResultsWindow();
            resultsWindow.DisplayResults(results);
            resultsWindow.ShowDialog();
        }
    }

    public List<object> SearchRecords(string criteria, string value)
    {
        List<object> results = new List<object>();

        switch (criteria)
        {
            case "Date":
                DateTime searchDate;
                if (DateTime.TryParse(value, out searchDate))
                {
                    results.AddRange(_incomes.Where(r => r.CreatedAt.Date == searchDate.Date).Cast<object>().ToList());
                    results.AddRange(_expenses.Where(r => r.CreatedAt.Date == searchDate.Date).Cast<object>().ToList());
                }
                break;
            case "Category":
                results.AddRange(_incomes.Where(r => r.IncomeSource.ToString().Contains(value, StringComparison.OrdinalIgnoreCase)).Cast<object>().ToList());
                results.AddRange(_expenses.Where(r => r.ExpenseSource.ToString().Contains(value, StringComparison.OrdinalIgnoreCase)).Cast<object>().ToList());
                break;
            case "Sum":
                decimal searchSum;
                if (decimal.TryParse(value, out searchSum))
                {
                    results.AddRange(_incomes.Where(r => r.Sum == searchSum).Cast<object>().ToList());
                    results.AddRange(_expenses.Where(r => r.Sum == searchSum).Cast<object>().ToList());
                }
                break;
            default:
                throw new Exception("Unknown search criteria");
        }
        return results;
    }
    
    private void ReportButton_Click(object sender, RoutedEventArgs e)
    {
        ReportWindow reportWindow = new ReportWindow(_incomes, _expenses);
        reportWindow.Show();
    }
    
    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        SortCriteriaWindow sortWindow = new SortCriteriaWindow();
        if (sortWindow.ShowDialog() == true)
        {
            if (sortWindow.IsSortingIncomes)
            {
                SortRecords<Income>(sortWindow.SelectedCriteria, _incomes, true);
            }
            else
            {
                SortRecords<Expense>(sortWindow.SelectedCriteria, _expenses, false);
            }
        }
    }

    public void SortRecords<T>(string criteria, List<T> records, bool isIncome) where T : class
    {
        switch (criteria)
        {
            case "Date":
                records = BuiltInSort(records, criteria);
                break;
            case "Category":
                records = BubbleSort(records, criteria);
                break;
            case "Sum":
                records = InsertionSort(records, criteria);
                break;
            default:
                throw new Exception("Unknown sorting criteria");
        }

        if (isIncome)
        {
            _incomes = records.Cast<Income>().ToList();
            IncomesListBox.ItemsSource = null;
            IncomesListBox.ItemsSource = _incomes;
        }
        else
        {
            _expenses = records.Cast<Expense>().ToList();
            ExpensesListBox.ItemsSource = null;
            ExpensesListBox.ItemsSource = _expenses;
        }
    }

    private List<T> BuiltInSort<T>(List<T> records, string criteria) where T : class
    {
        if (typeof(T) == typeof(Income))
        {
            var incomeRecords = records.Cast<Income>().ToList();
            return criteria switch
            {
                "Date" => incomeRecords.OrderBy(r => r.CreatedAt).ToList() as List<T>,
                _ => records
            };
        }
        else if (typeof(T) == typeof(Expense))
        {
            var expenseRecords = records.Cast<Expense>().ToList();
            return criteria switch
            {
                "Date" => expenseRecords.OrderBy(r => r.CreatedAt).ToList() as List<T>,
                _ => records
            };
        }
        return records;
    }

    private List<T> BubbleSort<T>(List<T> records, string criteria) where T : class
    {
        var length = records.Count;
        for (int i = 0; i < length - 1; i++)
        {
            for (int j = 0; j < length - i - 1; j++)
            {
                bool swap = false;
                if (typeof(T) == typeof(Income))
                {
                    var incomeRecords = records.Cast<Income>().ToList();
                    switch (criteria)
                    {
                        case "Category":
                            if (incomeRecords[j].IncomeSource > incomeRecords[j + 1].IncomeSource)
                                swap = true;
                            break;
                    }
                    if (swap)
                    {
                        var temp = incomeRecords[j];
                        incomeRecords[j] = incomeRecords[j + 1];
                        incomeRecords[j + 1] = temp;
                    }
                    records = incomeRecords as List<T>;
                }
                else if (typeof(T) == typeof(Expense))
                {
                    var expenseRecords = records.Cast<Expense>().ToList();
                    switch (criteria)
                    {
                        case "Category":
                            if (expenseRecords[j].ExpenseSource > expenseRecords[j + 1].ExpenseSource)
                                swap = true;
                            break;
                    }
                    if (swap)
                    {
                        var temp = expenseRecords[j];
                        expenseRecords[j] = expenseRecords[j + 1];
                        expenseRecords[j + 1] = temp;
                    }
                    records = expenseRecords as List<T>;
                }
            }
        }
        return records;
    }

    private List<T> InsertionSort<T>(List<T> records, string criteria) where T : class
    {
        if (typeof(T) == typeof(Income))
        {
            var incomeRecords = records.Cast<Income>().ToList();
            for (int i = 1; i < incomeRecords.Count; i++)
            {
                var key = incomeRecords[i];
                int j = i - 1;

                switch (criteria)
                {
                    case "Sum":
                        while (j >= 0 && incomeRecords[j].Sum > key.Sum)
                        {
                            incomeRecords[j + 1] = incomeRecords[j];
                            j--;
                        }
                        break;
                }
                incomeRecords[j + 1] = key;
            }
            records = incomeRecords as List<T>;
        }
        else if (typeof(T) == typeof(Expense))
        {
            var expenseRecords = records.Cast<Expense>().ToList();
            for (int i = 1; i < expenseRecords.Count; i++)
            {
                var key = expenseRecords[i];
                int j = i - 1;

                switch (criteria)
                {
                    case "Sum":
                        while (j >= 0 && expenseRecords[j].Sum > key.Sum)
                        {
                            expenseRecords[j + 1] = expenseRecords[j];
                            j--;
                        }
                        break;
                }
                expenseRecords[j + 1] = key;
            }
            records = expenseRecords as List<T>;
        }
        return records;
    }
}