using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;

namespace ExpenseTrackerClient;

public partial class FaildWindow : Window
{
    private TransactionsClient _httpClient;
    private const string FILE_PATH = "C:\\Users\\tonya\\Desktop\\ExpenseTrackerClient\\ExpenseTrackerClient\\UserAndAccountData.json";

    public FaildWindow()
    {
        InitializeComponent();
        _httpClient = new TransactionsClient();
    }
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow(_httpClient, FILE_PATH);
        registerWindow.ShowDialog();
    }
    
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow(_httpClient, FILE_PATH);
        loginWindow.ShowDialog();
    }
}