using System;
using System.Collections.Generic;
using Financify.Models;

namespace Financify.Models
{
    public class ExpenseListViewModel
    {
        public List<Expense> Expenses { get; set; } = new();

        // Filter
        public int Month { get; set; }   // 1–12
        public int Year { get; set; }    // e.g., 2025
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        // Summary
        public decimal Total { get; set; }
        public Dictionary<string, decimal> ByCategory { get; set; } = new();
    }
}
