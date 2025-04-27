using Financify.Data;
using Financify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Financify.Controllers
{
    [Authorize]
    public class SavingsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SavingsController(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Savings/Create
        public IActionResult Create()
        {
            ViewBag.ShowSuccess = false;
            return View("CreateSavings");
        }

        // POST: Savings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Savings savings)
        {
            Console.WriteLine("POST: Create action hit");

            var userId = _userManager.GetUserId(User);
            savings.UserId = userId;
            savings.DateSaved = DateTime.Now;

            _context.Savings.Add(savings);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Successfully saved");

            if (!string.IsNullOrEmpty(userId))
            {
                await UpdateStreakAsync(userId);
            }
            if (!string.IsNullOrEmpty(userId))
            {
                await CheckAndAwardBadgesAsync(userId);
            }

            ViewBag.SuccessMessage = "✅ Savings entry recorded!";
            ViewBag.ShowSuccess = true;
            ModelState.Clear();
            return View("CreateSavings", new Savings());
        }

        // GET: Savings/Track
        public IActionResult Track()
        {
            var userId = _userManager.GetUserId(User);

            var savings = _context.Savings
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.DateSaved)
                .ToList();

            var streak = _context.SavingsStreaks.FirstOrDefault(s => s.UserId == userId);
            var badges = _context.UserBadges
                .Where(b => b.UserId == userId)
                .Select(b => b.Badge)
                .ToList();

            ViewBag.TotalSaved = savings.Sum(s => s.Amount);
            ViewBag.CurrentStreak = streak?.CurrentStreak ?? 0;
            ViewBag.MaxStreak = streak?.MaxStreak ?? 0;
            ViewBag.Badges = badges;

            return View("TrackSavings", savings);
        }

        private async Task UpdateStreakAsync(string userId)
        {
            var streak = _context.SavingsStreaks.FirstOrDefault(s => s.UserId == userId);
            var today = DateTime.Today;

            if (streak == null)
            {
                streak = new SavingsStreak
                {
                    UserId = userId,
                    CurrentStreak = 1,
                    MaxStreak = 1,
                    LastSavedDate = today
                };
                _context.SavingsStreaks.Add(streak);
            }
            else
            {
                if (streak.LastSavedDate == today)
                    return;

                if (streak.LastSavedDate == today.AddDays(-1))
                    streak.CurrentStreak += 1;
                else
                    streak.CurrentStreak = 1;

                streak.MaxStreak = Math.Max(streak.MaxStreak, streak.CurrentStreak);
                streak.LastSavedDate = today;
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckAndAwardBadgesAsync(string userId)
        {
            var userTotal = _context.Savings.Where(s => s.UserId == userId).Sum(s => s.Amount);
            var streak = _context.SavingsStreaks.FirstOrDefault(s => s.UserId == userId)?.CurrentStreak ?? 0;
            var earnedBadgeIds = _context.UserBadges.Where(ub => ub.UserId == userId).Select(ub => ub.BadgeId).ToHashSet();

            var badges = _context.Badges.ToList();

            foreach (var badge in badges)
            {
                if (earnedBadgeIds.Contains(badge.BadgeId)) continue;

                if ((badge.ThresholdAmount.HasValue && userTotal >= badge.ThresholdAmount.Value) ||
                    (badge.StreakRequired.HasValue && streak >= badge.StreakRequired.Value))
                {
                    _context.UserBadges.Add(new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.BadgeId,
                        EarnedDate = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
