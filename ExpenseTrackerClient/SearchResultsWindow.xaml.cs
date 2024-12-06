namespace ExpenseTrackerClient;

public partial class SearchResultsWindow : System.Windows.Window
{
    private bool _searchIncome;
    private bool _searchExpense;
    private DateTime? _selectedDate;
    private decimal _selectedAmount;
    private string _selectedCategory;
    
    public SearchResultsWindow(bool searchIncome, bool searchExpense, DateTime? selectedDate, decimal selectedAmount, string selectedCategory)
    {
        InitializeComponent();
        _searchIncome = searchIncome;
        _searchExpense = searchExpense;
        _selectedDate = selectedDate;
        _selectedAmount = selectedAmount;
        _selectedCategory = selectedCategory;

        LoadResults();
    }
    
    private void LoadResults()
    {
        // Здесь добавьте логику для загрузки данных в зависимости от выбранных фильтров
        if (_searchIncome)
        {
            ResultsListBox.Items.Add($"Доходы: Зарплата | Дата: {_selectedDate} | Сумма: {_selectedAmount} | Категория: {_selectedCategory}");
            ResultsListBox.Items.Add($"Доходы: Премия | Дата: {_selectedDate} | Сумма: {_selectedAmount} | Категория: {_selectedCategory}");
        }

        if (_searchExpense)
        {
            ResultsListBox.Items.Add($"Расходы: Продукты | Дата: {_selectedDate} | Сумма: {_selectedAmount} | Категория: {_selectedCategory}");
            ResultsListBox.Items.Add($"Расходы: Коммунальные услуги | Дата: {_selectedDate} | Сумма: {_selectedAmount} | Категория: {_selectedCategory}");
        }
    }
}