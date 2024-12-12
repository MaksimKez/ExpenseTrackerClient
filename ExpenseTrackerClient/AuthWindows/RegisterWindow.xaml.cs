using System.IO;
using System.Windows;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;
using ExpenseTrackerClient.Models;
using Newtonsoft.Json;

namespace ExpenseTrackerClient;

public partial class RegisterWindow : Window
{
    private const string PATH = "C:\\Users\\Tonya\\Desktop\\kursClient\\ExpenseTrackerClient\\UserAndAccountData.json";

    public RegisterWindow()
    {
        InitializeComponent();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var clientU = new UserClient();
        var clientT = new TransactionsClient();
        string username = UsernameTextBox.Text;
        string password = PasswordBox.Password;

        // Проверяем, чтобы все поля были заполнены
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var bankAccountId = await clientT.CreateBankAccountAsync(new BankAccount()
        {
            Id = Guid.NewGuid(),
            Balance = 0
        });

        if (bankAccountId == Guid.Empty)
        {
            MessageBox.Show("Произошла ошибка при создании aккаунта. Введите корректные данные",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        var userId = await clientU.RegisterUserAsync(username, password, bankAccountId);
        
        if (userId == Guid.Empty) return;
            
        File.WriteAllText(PATH, JsonConvert.SerializeObject(new AccountData()
            {BankAccountId = bankAccountId }));
                
        MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK,
            MessageBoxImage.Information);

        Close();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}