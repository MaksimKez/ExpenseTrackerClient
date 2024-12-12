using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient.SortAndFindWindows;

public partial class SortWindow : Window
{
    public List<Income> _incomes;
    public List<Expense> _expenses;

    public SortWindow(ObservableCollection<Income> incomes, ObservableCollection<Expense> expenses)
    {
        _incomes = incomes.ToList();
        _expenses = expenses.ToList();
        InitializeComponent();
    }

    private void SortIncomesButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedCriteria = (criteriaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        if (selectedCriteria != null)
        {
            switch (selectedCriteria)
            {
                case "Дата":
                    _incomes = _incomes.OrderBy(income => income.CreatedAt).ToList();
                    break;
                case "Категория":
                    _incomes = _incomes.OrderBy(income => income.IncomeSource).ToList();
                    break;
                case "Сумма":
                    BubbleSort(_incomes);
                    break;
            }

            MessageBox.Show("Доходы успешно отсортированы!", "Сортировка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите критерий сортировки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SortExpensesButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedCriteria = (criteriaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        if (selectedCriteria != null)
        {
            switch (selectedCriteria)
            {
                case "Дата":
                    _expenses = _expenses.OrderBy(expense => expense.CreatedAt).ToList();
                    break;
                case "Категория":
                    _expenses = _expenses.OrderBy(expense => expense.ExpenseSource).ToList();
                    break;
                case "Сумма":
                    BubbleSort(_expenses);
                    break;
            }

            MessageBox.Show("Расходы успешно отсортированы!", "Сортировка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите критерий сортировки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void BubbleSort<T>(List<T> list) where T : class
    {
        if (typeof(T) == typeof(Income))
        {
            var incomeList = list.Cast<Income>().ToList();
            for (int i = 0; i < incomeList.Count - 1; i++)
            {
                for (int j = 0; j < incomeList.Count - i - 1; j++)
                {
                    if (incomeList[j].Sum > incomeList[j + 1].Sum)
                    {
                        (incomeList[j], incomeList[j + 1]) = (incomeList[j + 1], incomeList[j]);
                    }
                }
            }

            list.Clear();
            list.AddRange(incomeList.Cast<T>());
        }
        else if (typeof(T) == typeof(Expense))
        {
            var expenseList = list.Cast<Expense>().ToList();
            for (int i = 0; i < expenseList.Count - 1; i++)
            {
                for (int j = 0; j < expenseList.Count - i - 1; j++)
                {
                    if (expenseList[j].Sum > expenseList[j + 1].Sum)
                    {
                        (expenseList[j], expenseList[j + 1]) = (expenseList[j + 1], expenseList[j]);
                    }
                }
            }

            list.Clear();
            list.AddRange(expenseList.Cast<T>());
        }
    }
}
