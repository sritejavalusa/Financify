using Financify.Data;
using Financify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Financify.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExpenseController(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Expense/CreateExpense
        public IActionResult CreateExpense()
        {
            return View();
        }

        // POST: Expense/CreateExpense
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExpense(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                return View(expense);
            }

            var userId = _userManager.GetUserId(User);
            expense.UserId = userId;

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            ModelState.Clear(); // Clears the form after submission
            ViewBag.SuccessMessage = "✅ Expense added!";
            return View(); // Stay on same page
        }

        // GET: Expense/ViewExpenses
        public IActionResult ViewExpenses(int? month, int? year)
        {
            var userId = _userManager.GetUserId(User);

            var now = DateTime.UtcNow;
            int m = month ?? now.Month;
            int y = year ?? now.Year;

            // clamp invalids
            m = Math.Clamp(m, 1, 12);

            var expensesQuery = _context.Expenses
                .Where(e => e.UserId == userId
                            && e.Date.Year == y
                            && e.Date.Month == m);

            var expenses = expensesQuery
                .OrderByDescending(e => e.Date)
                .ToList();

            var byCategory = expenses
                .GroupBy(e => (e.Category ?? "Uncategorized").Trim())
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            var vm = new ExpenseListViewModel
            {
                Expenses = expenses,
                Month = m,
                Year = y,
                Total = expenses.Sum(e => e.Amount),
                ByCategory = byCategory
            };

            return View(vm);
        }

        // GET: Expense/EditExpense/{id}
        public IActionResult EditExpense(int id)
        {
            var expense = _context.Expenses.FirstOrDefault(e => e.ExpenseId == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expense/EditExpense
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExpense(int id, Expense expense)
        {
            if (id != expense.ExpenseId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return View(expense);

            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
            {
                return NotFound();
            }

            // ✅ Update only the editable fields
            existingExpense.Amount = expense.Amount;
            existingExpense.Category = expense.Category;
            existingExpense.Notes = expense.Notes;
            existingExpense.Date = expense.Date;

            // ❗ Don't touch UserId

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Expense updated!";
                return RedirectToAction(nameof(ViewExpenses));
            }
            catch
            {
                ModelState.AddModelError("", "❌ Failed to update expense.");
                return View(expense);
            }
        }

        // POST: Expense/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ViewExpenses));
        }
    }
}
