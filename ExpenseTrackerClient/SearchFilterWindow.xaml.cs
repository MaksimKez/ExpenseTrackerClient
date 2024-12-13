using System.Windows;
using System.Windows.Controls;
using DocumentFormat.OpenXml.Spreadsheet;
using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient;

public partial class SearchFilterWindow : Window
{ 
    private List<Income> _incomes; 
    private List<Expense> _expenses; 
    private List<object> _searchResults;
    
    public SearchFilterWindow() 
    { 
        InitializeComponent();
        _incomes = new List<Income>(); 
        _expenses = new List<Expense>(); 
    }

    private void ExitButton_Click(object send, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
     private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (criteriaComboBox.SelectedItem != null && !string.IsNullOrEmpty(searchValueTextBox.Text))
            {
                var selectedComboBoxItem = criteriaComboBox.SelectedItem as ComboBoxItem;
                var criteria = selectedComboBoxItem.Content.ToString();
                var value = searchValueTextBox.Text;
                _searchResults = SearchRecords(criteria, value);

                if (_searchResults != null && _searchResults.Any())
                {
                    SearchResultsWindow resultsWindow = new SearchResultsWindow();
                    resultsWindow.DisplayResults(_searchResults);
                    resultsWindow.Show();
                }
                else
                {
                    MessageBox.Show("Нет доступных результатов поиска. Пожалуйста, уточните критерий поиска.", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a search criteria and enter a search value.");
            }
        }

        private List<object> SearchRecords(string criteria, string value)
        {
            List<object> results = new List<object>();

            switch (criteria)
            {
                case "Date":
                    if (DateTime.TryParse(value, out DateTime searchDate))
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
                    if (decimal.TryParse(value, out decimal searchSum))
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
}