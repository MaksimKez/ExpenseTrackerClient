using System.IO;
using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;
using ExpenseTrackerClient.Data.Models.Dtos;
using ExpenseTrackerClient.Models;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ExpenseTrackerClient;

public partial class RegisterWindow : Window
{
    private TransactionsClient _clientT; 
    private readonly string path = "userAndAccountData.json";
    public RegisterWindow(TransactionsClient _client, string path)
    {
        _clientT = _client;
        this.path = path;
        InitializeComponent();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
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
        

        // Попытка входа пользователя
        var userId = await clientU.LoginUserAsync(username, password);

        if (userId == Guid.Empty)
        {
            MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    
        // Сохраняем данные пользователя в файл
        File.WriteAllText(path, JsonConvert.SerializeObject(new UserAndAccountData { UserId = userId }));

        MessageBox.Show("Вход выполнен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        this.Close();
    }
    
    private async void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}