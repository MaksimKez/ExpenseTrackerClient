using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient;

public partial class SearchResultsWindow : System.Windows.Window
{ 
    private List<Income> _incomes = new List<Income>();
    private List<Expense> _expenses = new List<Expense>();
    public SearchResultsWindow()
    { 
        InitializeComponent();
    }
    
    public void DisplayResults<T>(List<T> results) 
    { 
        ResultsListBox.ItemsSource = results;
    }
}