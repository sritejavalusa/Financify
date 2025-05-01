using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Financify.Models;

namespace Financify.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        // ✅ All your models registered here
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Savings> Savings { get; set; }
        public DbSet<SavingsStreak> SavingsStreaks { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }

        // ✅ Add Goal table
        public DbSet<Goal> Goals { get; set; }  // Add this line

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Budget Table Mapping
            builder.Entity<Budget>(b =>
            {
                b.ToTable("Budgets");
                b.HasKey(x => x.BudgetId);

                b.Property(x => x.FoodBudget).HasColumnType("decimal(18,2)");
                b.Property(x => x.HousingBudget).HasColumnType("decimal(18,2)");
                b.Property(x => x.EntertainmentBudget).HasColumnType("decimal(18,2)");
                b.Property(x => x.OtherBudget).HasColumnType("decimal(18,2)");
                b.Property(x => x.Income).HasColumnType("decimal(18,2)");

                b.HasOne(x => x.User)
                 .WithMany(u => u.Budgets!)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ Expense Table Mapping
            builder.Entity<Expense>(e =>
            {
                e.ToTable("Expenses");
                e.HasKey(x => x.ExpenseId);

                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");

                e.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ Savings Table Mapping
            builder.Entity<Savings>(s =>
            {
                s.ToTable("Savings");
                s.HasKey(x => x.Id);

                s.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                s.Property(x => x.DateSaved).IsRequired();

                s.HasOne<ApplicationUser>()
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ SavingsStreak Table Mapping
            builder.Entity<SavingsStreak>(ss =>
            {
                ss.ToTable("SavingsStreaks");
                ss.HasKey(x => x.Id);

                ss.Property(x => x.UserId).IsRequired();
                ss.Property(x => x.CurrentStreak).IsRequired();
                ss.Property(x => x.MaxStreak).IsRequired();
                ss.Property(x => x.LastSavedDate).IsRequired();
            });

            // ✅ Badge Table Mapping (FIXED warning here 👇)
            builder.Entity<Badge>(b =>
            {
                b.ToTable("Badges");
                b.HasKey(x => x.BadgeId);

                b.Property(x => x.Name).IsRequired();
                b.Property(x => x.Description).IsRequired();
                b.Property(x => x.IconUrl).IsRequired(false);

                // 👇 Fix for EF warning about decimal truncation
                b.Property(x => x.ThresholdAmount).HasColumnType("decimal(18,2)");
            });

            // ✅ UserBadge Table Mapping
            builder.Entity<UserBadge>(ub =>
            {
                ub.ToTable("UserBadges");
                ub.HasKey(x => x.Id);

                ub.HasOne(x => x.Badge)
                  .WithMany()
                  .HasForeignKey(x => x.BadgeId)
                  .OnDelete(DeleteBehavior.Cascade);

                ub.Property(x => x.UserId).IsRequired();
                ub.Property(x => x.EarnedDate).IsRequired();
            });

            // ✅ Goal Table Mapping
            builder.Entity<Goal>(g =>
            {
                g.ToTable("Goals");
                g.HasKey(x => x.GoalId);

                g.Property(x => x.TargetAmount).HasColumnType("decimal(18,2)").IsRequired();
                g.Property(x => x.CurrentAmount).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0);
                g.Property(x => x.GoalName).IsRequired();  // Using GoalName instead of Description
                g.Property(x => x.TargetDate).IsRequired();  // Using TargetDate instead of DueDate

                g.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
