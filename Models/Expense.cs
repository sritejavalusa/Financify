using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financify.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        public required string Category { get; set; }

        [Required]
        [Range(0,  100000, ErrorMessage = "Amount must be zero or positive")]
        public decimal Amount { get; set; }

        public string? Notes { get; set; }
    }
}
