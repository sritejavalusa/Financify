using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financify.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }

        [Required]
        public string? UserId { get; set; }  // Link to user

        [Required]
        public string? GoalName { get; set; }  // Example: "Save for new phone"

        [Required]
        public decimal TargetAmount { get; set; }  // Example: 15000 (amount to save)

        public decimal CurrentAmount { get; set; }  // Amount saved so far

        [Required]
        public DateTime TargetDate { get; set; }  // The goal deadline (e.g., by December 2025)

        // Automatically calculate the progress (for convenience in view)
        [NotMapped]
        public decimal ProgressPercentage
        {
            get
            {
                return (CurrentAmount / TargetAmount) * 100;
            }
        }

        // User navigation property
        public ApplicationUser? User { get; set; }
    }
}
