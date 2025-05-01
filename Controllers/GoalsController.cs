using Microsoft.AspNetCore.Mvc;
using Financify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Financify.Data;

namespace Financify.Controllers
{
    [Authorize]
    public class GoalsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoalsController(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action to view all goals for the current user
        public async Task<IActionResult> GoalsList()
        {
            var userId = _userManager.GetUserId(User);
            var goals = await _context.Goals.Where(g => g.UserId == userId).ToListAsync(); // Use _context.Goals here
            return View(goals);
        }

        // Action to create a new goal
        [HttpGet]
        public IActionResult CreateGoals()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoals(Goal goal)
        {
            if (ModelState.IsValid)
            {
                goal.UserId = _userManager.GetUserId(User);
                _context.Goals.Add(goal);  // Use _context.Goals here
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GoalsList));
            }
            return View(goal);
        }

        // Action to edit an existing goal
        [HttpGet]
        public async Task<IActionResult> EditGoals(int id)
        {
            var goal = await _context.Goals.FindAsync(id);  // Use _context.Goals here
            if (goal == null)
            {
                return NotFound();
            }
            return View(goal);
        }

        [HttpPost]
        public async Task<IActionResult> EditGoals(Goal goal)
        {
            if (ModelState.IsValid)
            {
                _context.Update(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GoalsList));
            }
            return View(goal);
        }

        // Action to track goal progress (update the saved amount)
        [HttpPost]
        public async Task<IActionResult> UpdateProgress(int goalId, decimal amount)
        {
            var goal = await _context.Goals.FindAsync(goalId);  // Use _context.Goals here
            if (goal != null)
            {
                goal.CurrentAmount += amount;
                _context.Update(goal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(GoalsList));
        }
    }
}
