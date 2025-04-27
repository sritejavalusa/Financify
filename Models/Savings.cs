using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financify.Models
{
    public class Savings
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        
        public DateTime DateSaved { get; set; }
    }

}
