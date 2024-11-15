using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using ExpenseTrackerClient.Data.Models;

namespace ExpenseTrackerClient
{
    /// <summary>
    /// Interaction logic for AddIncomeWindow.xaml
    /// </summary>
    public partial class AddIncomeWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        public static event Action<Income> IncomeAdded;
        public AddIncomeWindow()
        {
            InitializeComponent();
            DateTextBlock.Text = DateTime.Now.ToString("dd/mm/yyyy");
        }
        private async void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = (IncomeSourceEnum)Enum.Parse(typeof(IncomeSourceEnum), ((ComboBoxItem)CategoryBox.SelectedItem).Content.ToString());
            var sum = decimal.Parse(SumBox.Text);
            var title = TitleBox.Text;
            var date = DateTime.Now;

            var incomeDto = new Income
            {
                IncomeSource = selectedCategory,
                Sum = sum,
                Title = title,
                CreatedAt = date
            };

            try
            {
                // Отправляем данные на сервер
                var response = await client.PostAsJsonAsync("https://yourapiurl.com/api/income", incomeDto);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Income added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    IncomeAdded?.Invoke(incomeDto);
                    this.Close();
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to add income: {errorResponse}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            navigationService?.Navigate(new Uri("IncomeWindow.xaml", UriKind.Relative));
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            navigationService?.Navigate(new Uri("IncomeWindow.xaml", UriKind.Relative));
        }
    }
}
