namespace ExpenseTrackerClient.Data.Models;

public class Income
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal Sum { get; set; }
    public IncomeSourceEnum IncomeSource { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public bool IsValid() =>
        Id != Guid.Empty && !string.IsNullOrEmpty(Title) && Sum > 0
        && Enum.IsDefined(typeof(ExpenseSourceEnum), IncomeSource) && CreatedAt != DateTime.MinValue;

}