using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace zenra_finance_back.Models
{
    public class Expense
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("date")]
        public DateOnly Date { get; set; }

        [Required]
        [Column("expense_type")]
        public string ExpenseType { get; set; } = string.Empty;

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("created_at")]
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

    public class CurrentWeekExpenseResponse
    {
        public decimal Amount { get; set; }
    }
    public class CurrentMonthWeeklyIncomeExpenseResponse
    {
        public string Week { get; set; }
        public decimal AmountIncome { get; set; }
        public decimal AmountExpense { get; set; }
    }
}