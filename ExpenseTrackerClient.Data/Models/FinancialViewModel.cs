using System.ComponentModel;
namespace ExpenseTrackerClient.Data.Models;

public class FinancialViewModel
{
        private decimal totalIncome;
        public decimal TotalIncome
        {
            get { return totalIncome; }
            set
            {
                totalIncome = value;
                OnPropertyChanged(nameof(TotalIncome));
                OnPropertyChanged(nameof(Balance)); // Update balance when total income changes
            }
        }

        private decimal totalExpense;
        public decimal TotalExpense
        {
            get { return totalExpense; }
            set
            {
                totalExpense = value;
                OnPropertyChanged(nameof(TotalExpense));
                OnPropertyChanged(nameof(Balance)); // Update balance when total expense changes
            }
        }

        public decimal Balance => TotalIncome - TotalExpense;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public void AddIncome(decimal amount)
        {
            TotalIncome += amount;
        }

        public void AddExpense(decimal amount)
        {
            TotalExpense += amount;
        }

    }
    