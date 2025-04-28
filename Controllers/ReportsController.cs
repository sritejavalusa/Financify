using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Financify.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Financify.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore; // For logging

namespace Financify.Controllers
{
    [Authorize] // Ensures only authenticated users can access this controller
    public class ReportsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action to generate the category-wise graph with filter by month and year
        public async Task<IActionResult> CategoryWiseGraph(int? month, int? year)
        {
            // Fetch the logged-in user's ID
            var userId = _userManager.GetUserId(User); // Get the current logged-in user's ID
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if no user is found
            }

            // If month and year are not provided, return the view with a message
            if (!month.HasValue || !year.HasValue)
            {
                ViewBag.Message = "Please select both month and year to view the data.";
                return View();
            }

            // Fetch expenses for the logged-in user for the selected month and year
            var groupedData = await _context.Expenses
                                            .Where(e => e.UserId == userId && e.Date.Month == month.Value && e.Date.Year == year.Value) // Filter by userId, month, and year
                                            .GroupBy(e => e.Category) // Group expenses by category
                                            .Select(g => new
                                            {
                                                Category = g.Key, // Category name
                                                TotalAmount = g.Sum(e => e.Amount) // Sum of amounts in that category
                                            })
                                            .ToListAsync();

            // If no data is found, show a message
            if (!groupedData.Any())
            {
                ViewBag.Message = "No data available for the selected month and year.";
            }
            else
            {
                // Extract categories and amounts for chart rendering
                var categories = groupedData.Select(g => g.Category).ToList();
                var amounts = groupedData.Select(g => g.TotalAmount).ToList();

                // Pass the categories and amounts to the view using ViewBag
                ViewBag.Categories = categories;
                ViewBag.Amounts = amounts;
            }

            // Pass the selected month and year to the view for filtering
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;

            return View();
        }
    }
}
