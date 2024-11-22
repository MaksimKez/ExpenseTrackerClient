using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient;

public partial class AddExpenseWindow : Window
{
    private Guid _bankAccountId;
    private readonly TransactionsClient _client; 

    public AddExpenseWindow(Guid bankAccountId, TransactionsClient client)
    {
        _bankAccountId = bankAccountId;
        _client = client;
        
        InitializeComponent();
    }

    private async void OnSubmit(object sender, RoutedEventArgs e)
    {
        var expenseSourceStr = ExpenseTypeComboBox.SelectedItem.ToString();
        var expenseSource = new ExpenseSourceEnum();
        switch (expenseSourceStr)
        {
            case "System.Windows.Controls.ComboBoxItem: Аренда": expenseSource = ExpenseSourceEnum.Rent; break;
            case "System.Windows.Controls.ComboBoxItem: Продукты": expenseSource = ExpenseSourceEnum.Groceries; break;
            case "System.Windows.Controls.ComboBoxItem: Транспорт": expenseSource = ExpenseSourceEnum.Transport; break;
            case "System.Windows.Controls.ComboBoxItem: Развлечения": expenseSource = ExpenseSourceEnum.Entertainment; break;
            case "System.Windows.Controls.ComboBoxItem: Здоровье": expenseSource = ExpenseSourceEnum.HealthCare; break;
            case "System.Windows.Controls.ComboBoxItem: Образование": expenseSource = ExpenseSourceEnum.Education; break;
            case "System.Windows.Controls.ComboBoxItem: Другое": expenseSource = ExpenseSourceEnum.Miscellaneous; break;
            default: MessageBox.Show("Выберите тип дохода."); return;
        }

        var sum = decimal.Parse(SumTextBox.Text);
        
        var createdAt = CreatedAtDatePicker.SelectedDate ?? throw new ArgumentException("Выберите дату.");
        
        createdAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
        
        var expense = new Expense()
        {
            Id = Guid.NewGuid(),
            Title = TitleTextBox.Text,
            Sum = sum,
            ExpenseSource = expenseSource,
            CreatedAt = createdAt
        };
        
        var id = await _client.AddExpenseAsync(_bankAccountId, expense);
        
        if (id.Equals(Guid.Empty))
        {
            MessageBox.Show("Доход не был добавлен.");
            return;
        }
        
        MessageBox.Show("Доход был добавлен.");
        Close();
    }
    private async void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}