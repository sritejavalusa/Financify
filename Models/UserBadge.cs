namespace Financify.Models
{
    public class UserBadge
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int BadgeId { get; set; }
        public DateTime EarnedDate { get; set; }

        public Badge? Badge { get; set; }
    }
}