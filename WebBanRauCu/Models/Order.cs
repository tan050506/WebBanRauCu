using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations; // Thư viện validation
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanRauCu.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public int Status { get; set; } = 1; // 1: Đang xử lý

        // --- KIỂM TRA RỖNG ---
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string Address { get; set; }

        // --- KIỂM TRA SỐ & RỖNG ---
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (Phải bắt đầu bằng 0 và có 10-11 số)")]
        public string PhoneNumber { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        public List<OrderDetail>? OrderDetails { get; set; }
    }
}