using System.ComponentModel.DataAnnotations;

namespace WebBanRauCu.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]

        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
        public string? Address { get; set; }

    }
}
