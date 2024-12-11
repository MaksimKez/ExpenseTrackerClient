using System.Windows;

namespace ExpenseTrackerClient;

public partial class SearchFilterWindow : Window
{
    private string SelectedCategory { get; set; }
    
    public string SearchValue { get; private set; }
    public SearchFilterWindow()
    {
        InitializeComponent();
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        if (criteriaComboBox.SelectedItem != null && !string.IsNullOrEmpty(searchValueTextBox.Text)) 
        {
            SelectedCriteria = (criteriaComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            SearchValue = searchValueTextBox.Text;
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            MessageBox.Show("Please select a search criteria and enter a search value.");
        }
    }
}
