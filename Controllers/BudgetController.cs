using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Financify.Models;
using Financify.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Financify.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BudgetController(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // STEP 1: Ask for Month/Year
        public IActionResult Create()
        {
            if (TempData["ExistsMessage"] != null)
                ViewBag.ExistsMessage = TempData["ExistsMessage"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int month, int year)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                var existing = await _context.Budgets
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == month && b.Year == year);

                if (existing != null)
                {
                    TempData["ExistsMessage"] = $"⚠️ Budget already exists for {new DateTime(year, month, 1):MMMM yyyy}";
                    return RedirectToAction("Create");
                }

                TempData["TargetMonth"] = month;
                TempData["TargetYear"] = year;

                return RedirectToAction("CreateMonthly");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Create (POST): {ex.Message}");
                TempData["ExistsMessage"] = "Unexpected error occurred. Please try again.";
                return RedirectToAction("Create");
            }
        }

        // STEP 2: Budget entry for selected month/year
        public IActionResult CreateMonthly()
        {
            if (TempData["TargetMonth"] == null || TempData["TargetYear"] == null)
                return RedirectToAction("Create");

            if (TempData["TargetMonth"] != null && int.TryParse(TempData["TargetMonth"].ToString(), out int targetMonth))
                ViewBag.Month = targetMonth;
            else
                return RedirectToAction("Create");

            ViewBag.Year = (int)TempData["TargetYear"];

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMonthly(Budget budget)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(budget);
            }

            budget.UserId = userId;
            budget.User = user;

            if (!ModelState.IsValid)
            {
                return View(budget);
            }

            try
            {
                _context.Add(budget);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"✅ Budget created for {new DateTime(budget.Year, budget.Month, 1):MMMM yyyy}";
                TempData["TargetMonth"] = budget.Month;
                TempData["TargetYear"] = budget.Year;

                return RedirectToAction("CreateMonthly");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB ERROR: " + ex.Message);
                ModelState.AddModelError("", "❌ Failed to save budget to database.");
                return View(budget);
            }
        }

        // View Budgets by Month/Year
        public IActionResult ViewBudgets(int? month, int? year)
        {
            var userId = _userManager.GetUserId(User);
            var query = _context.Budgets.Where(b => b.UserId == userId);

            if (month.HasValue)
                query = query.Where(b => b.Month == month.Value);
            if (year.HasValue)
                query = query.Where(b => b.Year == year.Value);

            var result = query.ToList();

            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;

            return View(result);
        }

        // GET: Budget/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId);

            if (budget == null)
                return NotFound();

            return View("EditBudgets", budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Budget budget)
        {
            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                return View("EditBudgets", budget);
            }

            try
            {
                var existing = await _context.Budgets
                    .FirstOrDefaultAsync(b => b.BudgetId == budget.BudgetId && b.UserId == userId);

                if (existing == null)
                    return NotFound();

                existing.Income = budget.Income;
                existing.FoodBudget = budget.FoodBudget;
                existing.HousingBudget = budget.HousingBudget;
                existing.EntertainmentBudget = budget.EntertainmentBudget;
                existing.OtherBudget = budget.OtherBudget;

                _context.Update(existing);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = $"✅ Budget updated for {new DateTime(existing.Year, existing.Month, 1):MMMM yyyy}";

                // Redirect to GET Edit to display updated page with success message
                return RedirectToAction("Edit", new { id = existing.BudgetId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Edit failed: " + ex.Message);
                ModelState.AddModelError("", "❌ Failed to update budget.");
                return View("EditBudgets", budget);
            }
        }


        // POST: Budget/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId);

            if (budget == null)
                return NotFound();

            try
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"🗑️ Deleted budget for {new DateTime(budget.Year, budget.Month, 1):MMMM yyyy}";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete failed: " + ex.Message);
                TempData["ErrorMessage"] = "❌ Failed to delete the budget.";
            }

            return RedirectToAction("ViewBudgets");
        }
    }
}
