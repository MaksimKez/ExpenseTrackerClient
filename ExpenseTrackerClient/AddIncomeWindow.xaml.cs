using System.Windows;
using System.Windows.Controls;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient;

public partial class AddIncomeWindow : Window
{
    private Guid _bankAccountId;
    private readonly TransactionsClient _client; 
    
    public AddIncomeWindow(Guid bankAccountId, TransactionsClient client)
    {
        _bankAccountId = bankAccountId;
        _client = client;
        
        InitializeComponent();
    }

    private async void OnSubmit(object sender, RoutedEventArgs e)
    {
        var incomeSourceStr = IncomeSourceComboBox.SelectedItem.ToString();
        var incomeSource = new IncomeSourceEnum();
        switch (incomeSourceStr)
        {
            case "System.Windows.Controls.ComboBoxItem: Подарок": incomeSource = IncomeSourceEnum.Gift; break;
            case "System.Windows.Controls.ComboBoxItem: Зарплата": incomeSource = IncomeSourceEnum.Salary; break;
            case "System.Windows.Controls.ComboBoxItem: Процент от банка": incomeSource = IncomeSourceEnum.BankInterest; break;
            default: MessageBox.Show("Выберите тип дохода."); return;
        }

        var sum = decimal.Parse(SumTextBox.Text);
        
        var createdAt = CreatedAtDatePicker.SelectedDate ?? throw new ArgumentException("Выберите дату.");
        
        createdAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
        
        var income = new Income
        {
            Id = Guid.NewGuid(),
            Title = TitleTextBox.Text,
            Sum = sum,
            IncomeSource = incomeSource,
            CreatedAt = createdAt
        };
        
        var id = await _client.AddIncomeAsync(_bankAccountId, income);
    }
}