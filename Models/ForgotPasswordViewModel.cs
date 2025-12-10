using System.ComponentModel.DataAnnotations;

namespace Financify.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
