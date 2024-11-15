using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExpenseTrackerClient.Data.HttpClients;

namespace ExpenseTrackerClient
{
    /// <summary>
    /// Interaction logic for IncomeWindow.xaml
    /// </summary>
    public partial class IncomeWindow : Window
    {
        // делаешь клиент который я написал полем
        public TransactionsClient Client { get; set; }
        public IncomeWindow()
        {
            // объявляешь
            Client = new TransactionsClient();
            
            
            InitializeComponent();
        }
        
        //и используешь его так во всех окнах
        //todo ВАЖНО, посмотри сами классы, какие они параметры просят и что они возвращают
        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            navigationService?.Navigate(new Uri("MainWindow.xaml", UriKind.Relative));
        }
        private void Button_Click_Income(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            navigationService?.Navigate(new Uri("IncomeWindow.xaml", UriKind.Relative));
        }
        private void Button_Click_Expense(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            navigationService?.Navigate(new Uri("ExpenseWindow.xaml", UriKind.Relative));
        }
    }
}
