using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Financify.Models
{
    public class BudgetVsActualViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string? MonthName { get; set; }

        public decimal FoodBudget { get; set; }
        public decimal HousingBudget { get; set; }
        public decimal EntertainmentBudget { get; set; }
        public decimal OtherBudget { get; set; }

        public decimal TotalBudget => FoodBudget + HousingBudget + EntertainmentBudget + OtherBudget;

        public decimal FoodSpent { get; set; }
        public decimal HousingSpent { get; set; }
        public decimal EntertainmentSpent { get; set; }
        public decimal OtherSpent { get; set; }

        public decimal TotalSpent => FoodSpent + HousingSpent + EntertainmentSpent + OtherSpent;
    }
}
