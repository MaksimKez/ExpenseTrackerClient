using System.IO;
using System.Windows;
using ExpenseTrackerClient.Data.Models;
using OfficeOpenXml;

namespace ExpenseTrackerClient;

public partial class ReportWindow : Window
{
    private readonly List<Income> _incomes;
    private readonly List<Expense> _expenses;
    
    public ReportWindow(List<Income> incomes, List<Expense> expenses)
    {
        InitializeComponent();

        _incomes = incomes;
        _expenses = expenses;
        
        // Расчет сумм
        decimal totalSpent = expenses.Sum(e => e.Sum);
        decimal totalEarned = incomes.Sum(i => i.Sum);
        decimal balance = totalEarned - totalSpent;

        // Заполнение полей
        TotalSpentTextBlock.Text = $"Всего потрачено: {totalSpent:C}";
        TotalEarnedTextBlock.Text = $"Всего заработано: {totalEarned:C}";
        BalanceTextBlock.Text = $"Баланс: {balance:C}";
    }

    private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string filePath = $"Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            using (var package = new ExcelPackage())
            {
                // Создаем лист для доходов
                var incomeSheet = package.Workbook.Worksheets.Add("Доходы");
                incomeSheet.Cells[1, 1].Value = "Дата";
                incomeSheet.Cells[1, 2].Value = "Название";
                incomeSheet.Cells[1, 3].Value = "Категория";
                incomeSheet.Cells[1, 4].Value = "Сумма";

                for (int i = 0; i < _incomes.Count; i++)
                {
                    var income = _incomes[i];
                    incomeSheet.Cells[i + 2, 1].Value = income.CreatedAt.ToString("dd.MM.yyyy");
                    incomeSheet.Cells[i + 2, 2].Value = income.Title;
                    incomeSheet.Cells[i + 2, 3].Value = income.IncomeSource.ToString();
                    incomeSheet.Cells[i + 2, 4].Value = income.Sum;
                }

                // Создаем лист для расходов
                var expenseSheet = package.Workbook.Worksheets.Add("Расходы");
                expenseSheet.Cells[1, 1].Value = "Дата";
                expenseSheet.Cells[1, 2].Value = "Название";
                expenseSheet.Cells[1, 3].Value = "Категория";
                expenseSheet.Cells[1, 4].Value = "Сумма";

                for (int i = 0; i < _expenses.Count; i++)
                {
                    var expense = _expenses[i];
                    expenseSheet.Cells[i + 2, 1].Value = expense.CreatedAt.ToString("dd.MM.yyyy");
                    expenseSheet.Cells[i + 2, 2].Value = expense.Title;
                    expenseSheet.Cells[i + 2, 3].Value = expense.ExpenseSource.ToString();
                    expenseSheet.Cells[i + 2, 4].Value = expense.Sum;
                }

                // Сохраняем файл
                File.WriteAllBytes(filePath, package.GetAsByteArray());
            }

            MessageBox.Show($"Отчет успешно создан: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}