using Financify.Data;
using Financify.Models;
using System.Collections.Generic;
using System.Linq;


namespace Financify.Models
{
    public class Badge
    {
        public int BadgeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public decimal? ThresholdAmount { get; set; }
        public int? StreakRequired { get; set; }

        public static void Seed(AuthDbContext context)
        {
            if (!context.Badges.Any())
            {
                var badges = new List<Badge>
                {
                    new Badge { Name = "💵 First Saver", Description = "Saved your first amount!", ThresholdAmount = 1 },
                    new Badge { Name = "💯 Hundred Hero", Description = "Saved $100 total!", ThresholdAmount = 100 },
                    new Badge { Name = "🔥 7-Day Streak", Description = "Saved for 7 consecutive days!", StreakRequired = 7 },
                    new Badge { Name = "🏆 Streak King", Description = "30-Day Saving Streak!", StreakRequired = 30 },
                    new Badge { Name = "💰 Halfway There", Description = "Saved $500 total!", ThresholdAmount = 500 },
                    new Badge { Name = "💎 Saver Supreme", Description = "Saved $1000 total!", ThresholdAmount = 1000 }
                };

                context.Badges.AddRange(badges);
                context.SaveChanges();
            }
        }
    }
}
