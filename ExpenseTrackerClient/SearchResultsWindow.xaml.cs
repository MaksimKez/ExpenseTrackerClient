using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExpenseTrackerClient;

public partial class SearchResultsWindow : System.Windows.Window
{ 
    public SearchResultsWindow()
    {
        InitializeComponent();
    }

    public void DisplayResults<T>(List<T> results)
    {
        ResultsListBox.ItemsSource = results;
    }
}