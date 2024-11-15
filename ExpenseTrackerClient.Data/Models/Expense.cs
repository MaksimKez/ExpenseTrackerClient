namespace ExpenseTrackerClient.Data.Models;

public class Expense
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal Sum { get; set; }
    public ExpenseSourceEnum ExpenseSource { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public bool IsValid() =>
        Id != Guid.Empty && !string.IsNullOrEmpty(Title) && Sum > 0
               && Enum.IsDefined(typeof(ExpenseSourceEnum), ExpenseSource) && CreatedAt != DateTime.MinValue;
}