using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace zenra_finance_back.Models
{
    public class Finance
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public DateOnly Date { get; set; }
        [Required(ErrorMessage = "Income type is required")]
        public required string IncomeType { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }
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
}