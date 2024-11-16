using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;

namespace ExpenseTrackerClient;

public partial class DeleteExpenseWindow : Window
{
    private Guid _bankAccountId;
    private Guid _incomeId; 
    private readonly TransactionsClient _client; 

    public DeleteExpenseWindow(Guid incomeId, Guid bankAccountId, TransactionsClient client)
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
        var success = await _client.DeleteExpenseAsync(_incomeId, _bankAccountId);
        if (success)
        {
            MessageBox.Show("Расход успешно удален.");
            return;
        }
        
        MessageBox.Show("Произошла ошибка при удалении расхода.");
    }
}