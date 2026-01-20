using System.ComponentModel.DataAnnotations;

namespace WebBanRauCu.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)]
        public string Name { get; set; } 

        public int DisplayOrder { get; set; }
    }
}