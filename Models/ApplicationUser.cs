using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Financify.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }  // Nullable, it can be null if user doesn’t provide it
        public virtual ICollection<Budget> Budgets { get; set; }  // Navigation property
    }
}
