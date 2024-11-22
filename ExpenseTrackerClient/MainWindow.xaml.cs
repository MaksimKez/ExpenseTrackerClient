using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;
using ExpenseTrackerClient.Models;
using Newtonsoft.Json;

namespace ExpenseTrackerClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<Income> _incomes;
    private List<Expense> _expenses;
    private TransactionsClient _httpClient;
    private const string FILE_PATH = "C:\\Users\\prost\\Desktop\\ExpenseTrackerClient\\ExpenseTrackerClient\\UserAndAccountData.json";

    public MainWindow()
    {
        InitializeComponent();
        _incomes = new List<Income>();
        _expenses = new List<Expense>();
        _httpClient = new TransactionsClient();
    }
    
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow(_httpClient, FILE_PATH);
        registerWindow.ShowDialog();
    }
    
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow(_httpClient, FILE_PATH);
        loginWindow.ShowDialog();
    }
}