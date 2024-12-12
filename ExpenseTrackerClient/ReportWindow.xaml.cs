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
using System.Globalization;
using DocumentFormat.OpenXml.Packaging; 
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml.Core.ExcelPackage; // Не забудьте установить пакет EPPlus для работы с Excel

namespace ExpenseTrackerClient;

public partial class ReportWindow : System.Windows.Window
{
    private readonly FinancialViewModel _viewModel;
    public decimal TotalIncome => _incomes.Sum(i => i.Sum);
    public decimal TotalExpense => _expenses.Sum(e => e.Sum);
    public decimal Balance => TotalIncome - TotalExpense;
    
    private ObservableCollection<Income> _incomes;
    private ObservableCollection<Expense> _expenses;
    private TransactionsClient _httpClient;
    private const string FILE_PATH = "C:\\Users\\Tonya\\Desktop\\kursClient\\ExpenseTrackerClient\\UserAndAccountData.json";

    public ReportWindow(FinancialViewModel viewModel)
    {
        InitializeComponent();
        _incomes = new ObservableCollection<Income>();
        _expenses = new ObservableCollection<Expense>();
        _httpClient = new TransactionsClient();
        _viewModel = viewModel;
        DataContext = new FinancialViewModel();
    }
    
    private void ExitButton_Click(object send, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
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
                Row headerRow = new Row(); 
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
                            CellValue = new CellValue(income.CreatedAt), 
                            DataType = CellValues.String 
                        }, 
                        new Cell() 
                        {
                            CellValue = new CellValue(income.IncomeSource.ToString()), 
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
                            CellValue = new CellValue(expense.Sum),
                            DataType = CellValues.Number
                        },
                        new Cell() 
                        { 
                            CellValue = new CellValue(expense.CreatedAt),
                            DataType = CellValues.String 
                        },
                        new Cell()
                        {
                            // expense.ExpenseSource.ToString может не работать
                            // в случае чего поменять на свич, где создается стринга
                            CellValue = new CellValue(expense.ExpenseSource.ToString()),
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
