namespace Financify.Models
{
    public class SavingsStreak
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CurrentStreak { get; set; } // e.g. 5-day streak
        public int MaxStreak { get; set; }
        public DateTime LastSavedDate { get; set; }
    }
}