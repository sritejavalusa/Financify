using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financify.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        // ✅ Not [Required] because it will be assigned in the controller, not user input
        [ForeignKey("User")]
        public string? UserId { get; set; }

        // ✅ Will be injected by controller — no need to validate via form
        public ApplicationUser? User { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public decimal Income { get; set; }

        [Required]
        public decimal FoodBudget { get; set; }

        [Required]
        public decimal HousingBudget { get; set; }

        [Required]
        public decimal EntertainmentBudget { get; set; }

        [Required]
        public decimal OtherBudget { get; set; }

        // ✅ Automatically calculated; not stored in DB
        [NotMapped]
        public decimal TotalBudget => FoodBudget + HousingBudget + EntertainmentBudget + OtherBudget;

        // ✅ Safely calculates the month name
        [NotMapped]
        public string MonthName
        {
            get
            {
                if (Month >= 1 && Month <= 12 && Year >= 1)
                {
                    try
                    {
                        return new DateTime(Year, Month, 1).ToString("MMMM");
                    }
                    catch { }
                }
                return "Invalid Date";
            }
        }
    }
}
