using System.ComponentModel.DataAnnotations;

namespace WebBanRauCu.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password là bắt buộc")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
