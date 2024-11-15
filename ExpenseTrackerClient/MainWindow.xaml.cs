using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpenseTrackerClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void Button_Click_Registration(object sender, RoutedEventArgs e)
    {
        NavigationService navigationService = NavigationService.GetNavigationService(this);
        navigationService?.Navigate(new Uri("RegistrationWindow.xaml", UriKind.Relative));
        this.Close();
    }
    private void Button_Click_Auth(object sender, RoutedEventArgs e)
    {
        NavigationService navigationService = NavigationService.GetNavigationService(this);
        navigationService?.Navigate(new Uri("LoginWindow.xaml", UriKind.Relative));
        this.Close();
    }
}