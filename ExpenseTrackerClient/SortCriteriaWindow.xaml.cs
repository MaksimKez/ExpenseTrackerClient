using System.Windows;

namespace ExpenseTrackerClient;

public partial class SortCriteriaWindow : System.Windows.Window
{
    public string SelectedCriteria { get; private set; }
    
    public bool IsSortingIncomes { get; private set; }
    
    public SortCriteriaWindow()
    {
        InitializeComponent();
    }
    
    private void SortIncomesButton_Click(object sender, RoutedEventArgs e)
    {
        if (criteriaComboBox.SelectedItem != null)
        {
            SelectedCriteria = (criteriaComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            IsSortingIncomes = true;
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            MessageBox.Show("Please select a sort criteria.");
        }
    }
    
    private void SortExpensesButton_Click(object sender, RoutedEventArgs e)
    {
        if (criteriaComboBox.SelectedItem != null)
        {
            SelectedCriteria = (criteriaComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            IsSortingIncomes = false;
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            MessageBox.Show("Please select a sort criteria.");
        }
    }
}