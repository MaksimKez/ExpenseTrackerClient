using System.Windows;

namespace ExpenseTrackerClient;

public partial class SearchFilterWindow : Window
{
    private string SelectedCategory { get; set; }
    public SearchFilterWindow()
    {
        InitializeComponent();
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        bool searchIncome = IncomeCheckBox.IsChecked ?? false;
        bool searchExpense = ExpenseCheckBox.IsChecked ?? false;
        bool searchByDate = DateCheckBox.IsChecked ?? false;
        bool searchByAmount = AmountCheckBox.IsChecked ?? false;
        bool searchByCategory = CategoryCheckBox.IsChecked ?? false;

        DateTime? selectDate = searchByDate ? DatePicker.SelectedDate : null;
        decimal selectedAmount = searchByAmount ? decimal.Parse(AmountTextBox.Text) : 0;

        SearchResultWindow resultWindow = new SearchResultWindow(searchIncome, searchExpense, selectDate,
            selectedAmount, this.SelectedCategory);
        resultWindow.Show();
        this.Close();
    }

    private void SelectCategoryButton_Click(object sender, RoutedEventArgs e)
    { 
        CategorySelectionWindow categoryWindow = new CategorySelectionWindow(IncomeCheckBox.IsChecked ?? false || ExpenseCheckBox.IsChecked ?? false;); 
        if (categoryWindow.ShowDialog() == true) 
        { 
            // Логика для получения выбранной категории из categoryWindow
            string selectedCategory = categoryWindow.SelectedCategory; 
            this.SelectedCategory = selectedCategory;
                
        } 
    }
}
