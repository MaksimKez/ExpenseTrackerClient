using System.Windows;
using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient;

public partial class ReportWindow : Window
{
    public ReportWindow(List<Income> incomes, List<Expense> expenses)
    {
        InitializeComponent();

        // Расчет сумм
        decimal totalSpent = expenses.Sum(e => e.Sum);
        decimal totalEarned = incomes.Sum(i => i.Sum);
        decimal balance = totalEarned - totalSpent;

        // Заполнение полей
        TotalSpentTextBlock.Text = $"Всего потрачено: {totalSpent:C}";
        TotalEarnedTextBlock.Text = $"Всего заработано: {totalEarned:C}";
        BalanceTextBlock.Text = $"Баланс: {balance:C}";
    }

    private void ThrowExceptionButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException("Доделать!!!");
    }
}