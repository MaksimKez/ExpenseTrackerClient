using System.IO;
using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Models;
using Newtonsoft.Json;

namespace ExpenseTrackerClient;

public partial class LogInWindow : Window
{ 
    private const string PATH = "C:\\Users\\prost\\Desktop\\ExpenseTrackerClient\\ExpenseTrackerClient\\UserAndAccountData.json";
    
    public LogInWindow()
    {
        InitializeComponent();
    }

    private async void LogInButton_Click(object sender, RoutedEventArgs e)
    {
        var userClient = new UserClient();
        
        string username = UsernameTextBox.Text;
        string password = PasswordBox.Password;

        var bankAccountId = await userClient.LoginUserAsync(username, password);


        if (bankAccountId.Equals(Guid.Empty))
        {
            MessageBox.Show("Неверный логин или пароль.");
            return;
        }

        await File.WriteAllTextAsync(PATH, JsonConvert.SerializeObject(new AccountData {BankAccountId = bankAccountId }));

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}