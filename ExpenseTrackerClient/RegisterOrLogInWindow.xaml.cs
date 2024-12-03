using System.Windows;

namespace ExpenseTrackerClient;

public partial class RegisterOrLogInWindow : Window
{
    public RegisterOrLogInWindow()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var logInWindow = new LogInWindow();
        logInWindow.Show();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow();
        registerWindow.Show();
    }
}