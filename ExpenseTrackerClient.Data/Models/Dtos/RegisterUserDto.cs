namespace ExpenseTrackerClient.Data.Models.Dtos;

public class RegisterUserDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Guid BankAccountId { get; set; }
    
    public bool IsValid() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
}