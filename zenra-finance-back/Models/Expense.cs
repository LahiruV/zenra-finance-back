using System.ComponentModel.DataAnnotations;

namespace zenra_finance_back.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Date is required")]
        public DateOnly Date { get; set; }
        [Required(ErrorMessage = "Expense type is required")]
        public required string ExpenseType { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class MonthExpenseResponse
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }

    public class CurrentWeekDailyExpenseResponse
    {
        public string Day { get; set; }
        public decimal Amount { get; set; }
    }

    public class CurrentWeekDailyIncomeExpenseResponse
    {
        public string Day { get; set; }
        public decimal AmountIncome { get; set; }
        public decimal AmountExpense { get; set; }
    }

    public class MonthIncomeExpenseResponse
    {
        public string Month { get; set; }
        public decimal AmountIncome { get; set; }
        public decimal AmountExpense { get; set; }
    }
}