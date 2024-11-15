using System.Net.Http;
using System.Windows;
using System.Windows.Navigation;
using ExpenseTrackerClient.Data.Models.Dtos;


namespace ExpenseTrackerClient
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private static readonly HttpClient _httpclient = new HttpClient();
        public RegistrationWindow()
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
            var username = UserNameBox.Text;
            var userpassword = UserPasswordBox.Text;

            var registrationDto = new RegisterUserDto
            {
                Username = username,
                Password = userpassword
            };

            try
            {
                // Отправляем данные на сервер
                var response = await client.PostAsJsonAsync("https://yourapiurl.com/api/register", registrationDto);

                if (response.IsSuccessStatusCode)
                {
                    // Успешная регистрация
                    MessageBox.Show("Registration successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Навигация на другую страницу или окно
                    NavigationService navigationService = NavigationService.GetNavigationService(this);
                    navigationService?.Navigate(new Uri("LoginWindow.xaml", UriKind.Relative));
                    this.Close();
                }
                else
                {
                    // Ошибка регистрации
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Registration failed: {errorResponse}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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