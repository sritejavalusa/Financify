using System.ComponentModel.DataAnnotations;

namespace Financify.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        //[Display(Name = "User ID")]
        //[RegularExpression("^[a-zA-Z]+$", ErrorMessage = "User ID should contain only alphabets")]
        
        [Display(Name = "User ID")]
        public required string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
