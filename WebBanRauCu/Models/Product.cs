using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanRauCu.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; } // Mô tả có thể null

        [Required]
        public double Price { get; set; } // Giá bán

        public string? ImageUrl { get; set; } // Đường dẫn ảnh

        // Khóa ngoại liên kết với Category
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}