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
        // Create SaveFileDialog to get the file path
        var saveFileDialog = new SaveFileDialog
        { 
            Filter = "Excel files (*.xlsx)|*.xlsx", 
            FileName = "Отчет"
        };
        
        if (saveFileDialog.ShowDialog() == true)
        { 
            var filePath = saveFileDialog.FileName; 
            // Create and populate the Excel file
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, 
                       DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook)) 
            { 
                WorkbookPart workbookPart = document.AddWorkbookPart(); 
                workbookPart.Workbook = new Workbook(); 
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>(); 
                worksheetPart.Worksheet = new Worksheet(new SheetData());
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets()); 
                Sheet sheet = new Sheet() 
                { 
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart), 
                    SheetId = 1, 
                    Name = "Доходы и Расходы" 
                }; 
                
                sheets.Append(sheet); 
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                
                // Add header
                row Row headerRow = new Row(); 
                headerRow.Append( new Cell()
                {
                    CellValue = new CellValue("Категория"),
                     DataType = CellValues.String
                }, 
                    new Cell() 
                    { 
                        CellValue = new CellValue("Сумма"), 
                        DataType = CellValues.String 
                    }, 
                    new Cell() 
                    { 
                        CellValue = new CellValue("Дата"), 
                        DataType = CellValues.String 
                    }, 
                    new Cell() 
                    { 
                        CellValue = new CellValue("Тип"),
                        DataType = CellValues.String
                    }, 
                    new Cell() 
                    { 
                        CellValue = new CellValue("Комментарий"), 
                        DataType = CellValues.String 
                    }
                    );
                sheetData.Append(headerRow); 
                
                // Add income data
                foreach (var income in _incomes) 
                { 
                    Row row = new Row(); 
                    row.Append( new Cell() 
                        { 
                            CellValue = new CellValue("Доход"), 
                            DataType = CellValues.String 
                        }, 
                        new Cell()
                        {
                            CellValue = new CellValue(income.Sum.ToString()),
                            DataType = CellValues.Number 
                        }, 
                        new Cell() 
                        { 
                            CellValue = new CellValue(income.Date), 
                            DataType = CellValues.String 
                        }, 
                        new Cell() 
                        { 
                            CellValue = new CellValue(income.IncomeSource), 
                            DataType = CellValues.String 
                        }, 
                        new Cell()
                        {
                            CellValue = new CellValue(income.Title), 
                            DataType = CellValues.String 
                        }
                        ); 
                    sheetData.Append(row); 
                } 
                
                // Add expense data
                foreach (var expense in _expenses)
                {
                    Row row = new Row();
                    row.Append( new Cell()
                        { 
                            CellValue = new CellValue("Расход"),
                            DataType = CellValues.String
                        },
                        new Cell()
                        {
                            CellValue = new CellValue(expense.Sum.ToString()),
                            DataType = CellValues.Number
                        },
                        new Cell() 
                        { 
                            CellValue = new CellValue(expense.Date),
                            DataType = CellValues.String 
                        },
                        new Cell()
                        {
                            CellValue = new CellValue(expense.ExpenseSource),
                            DataType = CellValues.String 
                        },
                        new Cell()
                        {
                            CellValue = new CellValue(expense.Title),
                            DataType = CellValues.String
                        }
                    );
                    sheetData.Append(row);
                } 
            }
            MessageBox.Show("File saved successfully!");
        }
    }
}
