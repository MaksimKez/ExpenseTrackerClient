using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;

namespace ExpenseTrackerClient;

public partial class DeleteIncomeWindow : Window
{
    private Guid _bankAccountId;
    private Guid _incomeId; 
    private readonly TransactionsClient _client; 

    public DeleteIncomeWindow(Guid incomeId, Guid bankAccountId, TransactionsClient client)
    {
        _bankAccountId = bankAccountId;
        _client = client;
        _incomeId = incomeId;
        
        InitializeComponent();
    }

    private void OnNoClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void OnYesClick(object sender, RoutedEventArgs e)
    {
        var success = await _client.DeleteIncomeAsync(_incomeId, _bankAccountId);
        if (success)
        {
            MessageBox.Show("Доход успешно удален.");
            return;
        }
        
        MessageBox.Show("Произошла ошибка при удалении дохода.");
        this.Close();
    }
}