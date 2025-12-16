using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace zenra_finance_back.Models
{
    public class Finance
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
        [Column("income_type")]
        public string IncomeType { get; set; } = string.Empty;

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class MonthFinanceResponse
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }

    public class YearFinanceResponse
    {
        public string Year { get; set; }
        public decimal Amount { get; set; }
    }

    public class CurrentWeekDailyFinanceResponse
    {
        public string Day { get; set; }
        public decimal Amount { get; set; }
    }

    public class CurrentWeekFinanceResponse
    {
        public decimal Amount { get; set; }
    }

}