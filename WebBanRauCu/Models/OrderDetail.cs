using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanRauCu.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Quantity { get; set; } // Số lượng mua

        public double Price { get; set; } // Giá tại thời điểm mua
    }
}