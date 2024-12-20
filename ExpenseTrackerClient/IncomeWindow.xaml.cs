﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ExpenseTrackerClient.Data.Models;
using ExpenseTrackerClient.Data.HttpClients;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace ExpenseTrackerClient
{
    /// <summary>
    /// Interaction logic for IncomeWindow.xaml
    /// </summary>
    private static readonly HttpClient client = new HttpClient();
    public partial class IncomeWindow : Window
    {
        private ObservableCollection<Income> _incomes = new ObservableCollection<Income>();
        // делаешь клиент который я написал полем
        public TransactionsClient Client { get; set; }
        public IncomeWindow()
        {
            // объявляешь
            Client = new TransactionsClient();


            InitializeComponent();
            IncomeListBox.ItemsSource = _incomes;
            AddIncomeWindow.IncomeAdded += OnIncomeAdded;


        }

        //и используешь его так во всех окнах
        //todo ВАЖНО, посмотри сами классы, какие они параметры просят и что они возвращают
        private void Button_Click_AddIncome(object sender, RoutedEventArgs e)
        {
            var addIncomeWindow = new AddIncomeWindow();
            addIncomeWindow.ShowDialog();
        }

        private void OnIncomeAdded(Income income)
        {
            _incomes.Add(income);
        }
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

        private async void DeleteIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var incomeId = (Guid)button.Tag;

            // Удаляем доход из списка
            var incomeToDelete = _incomes.FirstOrDefault(i =>
            {
                return i.Id == incomeId;
            });
            if (incomeToDelete != null)
            {
                _incomes.Remove(incomeToDelete);

                // Отправляем запрос на сервер для удаления дохода из базы данных
                var response = await client.DeleteAsync($"https://yourapiurl.com/api/income/{incomeId}");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to delete income from server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

