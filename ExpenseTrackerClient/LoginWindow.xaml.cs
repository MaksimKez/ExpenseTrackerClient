using System.IO;
using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;
using ExpenseTrackerClient.Data.Models.Dtos;
using ExpenseTrackerClient.Models;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ExpenseTrackerClient;

public partial class LoginWindow : Window
{
    private TransactionsClient _clientT; 
    private readonly string path = "userAndAccountData.json";
    public LoginWindow(TransactionsClient _client, string path)
    {
        _clientT = _client;
        this.path = path;
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var clientU = new UserClient();
        string username = UsernameTextBox.Text;
        string password = PasswordBox.Password;

        // Проверяем, чтобы все поля были заполнены
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Создаем DTO с введенными данными
        LoginUserDto newUser = new LoginUserDto
        {
            Username = username,
            Password = password
        };

        var bankAccountId = await _clientT.CreateBankAccountAsync(new BankAccount()
        {
            Id = Guid.NewGuid(),
            Balance = 0
        });

        if (bankAccountId == Guid.Empty) return;
        
        var userId = await clientU.RegisterUserAsync(username, password, bankAccountId);
        
        if (userId == Guid.Empty) return;
            
        File.WriteAllText(path, JsonConvert.SerializeObject(new UserAndAccountData
            { UserId = userId, BankAccountId = bankAccountId }));
                
        MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK,
            MessageBoxImage.Information);
        
        HomeWindow homeWindow = new HomeWindow(); 
        homeWindow.Show(); 
        this.Close();
    }

    private async void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}