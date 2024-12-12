using System.Windows;
using System.Windows.Controls;
using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient.SortAndSearchWindows;

    public partial class SearchWindow : Window
    {
        private List<Income> _incomes;
        private List<Expense> _expenses;

        public SearchWindow(List<Income> incomes, List<Expense> expenses)
        {
            _incomes = incomes;
            _expenses = expenses;
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            resultsListBox.Items.Clear();

            var selectedCriteria = (criteriaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var value = valueTextBox.Text;

            if (string.IsNullOrWhiteSpace(value) || selectedCriteria == null)
            {
                MessageBox.Show("Пожалуйста, выберите критерий и введите значение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedCriteria == "Дата")
            {
                if (DateTime.TryParse(value, out DateTime date))
                {
                    var incomeResults = _incomes.Where(income => income.CreatedAt.Date == date.Date);
                    var expenseResults = _expenses.Where(expense => expense.CreatedAt.Date == date.Date);

                    AddResultsToListBox(incomeResults, expenseResults);
                }
                else
                {
                    MessageBox.Show("Некорректный формат даты. Используйте формат ГГГГ-ММ-ДД.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (selectedCriteria == "Сумма")
            {
                if (decimal.TryParse(value, out decimal sum))
                {
                    var incomeResults = _incomes.Where(income => income.Sum == sum);
                    var expenseResults = _expenses.Where(expense => expense.Sum == sum);

                    AddResultsToListBox(incomeResults, expenseResults);
                }
                else
                {
                    MessageBox.Show("Некорректный формат суммы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddResultsToListBox(IEnumerable<Income> incomeResults, IEnumerable<Expense> expenseResults)
        {
            foreach (var income in incomeResults)
            {
                resultsListBox.Items.Add($"Доход: {income.Title}, Сумма: {income.Sum}, Дата: {income.CreatedAt}");
            }

            foreach (var expense in expenseResults)
            {
                resultsListBox.Items.Add($"Расход: {expense.Title}, Сумма: {expense.Sum}, Дата: {expense.CreatedAt}");
            }

            if (!incomeResults.Any() && !expenseResults.Any())
            {
                MessageBox.Show("Результаты не найдены.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
