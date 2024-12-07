using System.Windows;

namespace ExpenseTrackerClient;

public partial class SortCriteriaWindow : System.Windows.Window
{
    public string SelectedCriteria { get; private set; }
    
    public SortCriteriaWindow()
    {
        InitializeComponent();
    }
    
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (DateRadioButton.IsChecked == true)
        {
            SelectedCriteria = "Date";
        }
        /*else if (CategoryRadioButton.IsChecked == true)
        {
            SelectedCriteria = "Category";
        }*/
        else if (AmountRadioButton.IsChecked == true)
        {
            SelectedCriteria = "Amount";
        }

        this.DialogResult = true;
    }
}