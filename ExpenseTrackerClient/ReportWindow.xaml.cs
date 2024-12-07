using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using OfficeOpenXml; // Не забудьте установить пакет EPPlus для работы с Excel

namespace ExpenseTrackerClient;

public partial class ReportWindow : System.Windows.Window
{
    public decimal TotalIncome => _incomes.Sum(i => i.Sum);
    public decimal TotalExpense => _expenses.Sum(e => e.Sum);
    public decimal Balance => TotalIncome - TotalExpense;
    
    private ObservableCollection<Income> _incomes;
    private ObservableCollection<Expense> _expenses;
    private TransactionsClient _httpClient;
    private const string FILE_PATH = "C:\\Users\\Tonya\\Desktop\\kursClient\\ExpenseTrackerClient\\UserAndAccountData.json";

    public ReportWindow()
    {
        InitializeComponent();
        _incomes = new ObservableCollection<Income>();
        _expenses = new ObservableCollection<Expense>();
        _httpClient = new TransactionsClient();
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
    
    private void ReportButton_Click(object sender, RoutedEventArgs e)
    {
        GenerateReport();
    } 
    private void GenerateReport()
    { 
        using (var package = new ExcelPackage())
        { 
            var worksheet = package.Workbook.Worksheets.Add("Доходы и Расходы"); 
            
            // Заголовки
            worksheet.Cells[1, 1].Value = "Категория";
            worksheet.Cells[1, 2].Value = "Сумма";
            worksheet.Cells[1, 3].Value = "Дата";
            worksheet.Cells[1, 4].Value = "Тип"; 
            worksheet.Cells[1, 5].Value = "Комментарий";

            // Данные доходов
            int row = 1;
            foreach (var income in _incomes)
            {
                worksheet.Cells[row, 1].Value = "Доход";
                worksheet.Cells[row, 2].Value = income.Sum;
                worksheet.Cells[row, 3].Value = income.Date;
                worksheet.Cells[row, 4].Value = income.IncomeSource;
                worksheet.Cells[row, 5].Value = income.Title; 
                row++;
            }

            // Данные расходов
            foreach (var expense in _expenses)
            {
                worksheet.Cells[row, 1].Value = "Расход";
                worksheet.Cells[row, 2].Value = expense.Sum;
                worksheet.Cells[row, 3].Value = expense.Date;
                worksheet.Cells[row, 4].Value = expense.ExpenseSource;
                worksheet.Cells[row, 5].Value = expense.Title; 
                row++;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                FileName = "Отчет"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var file = new FileInfo(saveFileDialog.FileName);
                package.SaveAs(file);
            }
        }
    }
}