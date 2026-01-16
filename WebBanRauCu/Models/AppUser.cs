using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebBanRauCu.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        public string? Address { get; set; }
    }
}
