using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpenseTrackerClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        using ExpenseTrackerClient.Data.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ExpenseTrackerClient
    {
        public partial class LoginWindow : Window
        {
            private static readonly HttpClient client = new HttpClient();

            public LoginWindow()
            {
                InitializeComponent();
            }
            private void Button_Click_Back(object sender, RoutedEventArgs e)
            {
                NavigationService navigationService = NavigationService.GetNavigationService(this);
                navigationService?.Navigate(new Uri("MainWindow.xaml", UriKind.Relative));
            }
            private async void Button_Click_Regitrate(object sender, RoutedEventArgs e)
            {
                var username = UserNameTextBox.Text;
                var userpassword = UserPasswordTextBox.Text;

                // Создаем объект для отправки на сервер
                var loginDto = new LoginUserDto
                {
                    Username = username,
                    Password = userpassword
                };

                try
                {
                    // Отправляем данные на сервер
                    var response = await client.PostAsJsonAsync("https://yourapiurl.com/api/login", loginDto);

                    if (response.IsSuccessStatusCode)
                    {
                        // Успешный вход
                        MessageBox.Show("Login successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Навигация на основное окно или другую страницу
                        NavigationService navigationService = NavigationService.GetNavigationService(this);
                        navigationService?.Navigate(new Uri("IncomeWindow.xaml", UriKind.Relative));
                        this.Close();
                    }
                    else
                    {
                        // Ошибка входа
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Login failed: {errorResponse}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Обработка исключений
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
}
