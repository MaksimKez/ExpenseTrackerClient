using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;
using ExpenseTrackerClient.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging; 
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml.Core.ExcelPackage; // Не забудьте установить пакет EPPlus для работы с Excel

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
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            FileName = "Отчет"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            var file = new FileInfo(saveFileDialog.FileName);

            using (var package = new ExcelPackage(file))
            { 
                var worksheet = package.Workbook.Worksheets.Add("Доходы и Расходы"); 
                
                worksheet.Cells[1, 1].Value = "Категория";
                worksheet.Cells[1, 2].Value = "Сумма";
                worksheet.Cells[1, 3].Value = "Дата";
                worksheet.Cells[1, 4].Value = "Тип"; 
                worksheet.Cells[1, 5].Value = "Комментарий"; //Install-Package EPPlus

                int row = 2; // Start from the second row

                foreach (var income in _incomes)
                {
                    worksheet.Cells[row, 1].Value = "Доход";
                    worksheet.Cells[row, 2].Value = income.Sum;
                    worksheet.Cells[row, 3].Value = income.Date;
                    worksheet.Cells[row, 4].Value = income.IncomeSource;
                    worksheet.Cells[row, 5].Value = income.Title; 
                    row++;
                }

                foreach (var expense in _expenses)
                {
                    worksheet.Cells[row, 1].Value = "Расход";
                    worksheet.Cells[row, 2].Value = expense.Sum;
                    worksheet.Cells[row, 3].Value = expense.Date;
                    worksheet.Cells[row, 4].Value = expense.ExpenseSource;
                    worksheet.Cells[row, 5].Value = expense.Title; 
                    row++;
                }

                try
                {
                    package.Save();
                    MessageBox.Show("File saved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}");
                }
            }
        }
    }
}
