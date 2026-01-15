using Microsoft.AspNetCore.Mvc;
using WebBanRauCu.Models;
using WebBanRauCu.Helpers; // Nhớ dùng namespace này

namespace WebBanRauCu.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Xem giỏ hàng
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang") ?? new List<CartItem>();
            return View(cart);
        }

        // Thêm vào giỏ hàng
        public IActionResult AddToCart(int id)
        {
            // Lấy danh sách giỏ hàng hiện tại từ Session
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang") ?? new List<CartItem>();

            // Kiểm tra xem sản phẩm đã có trong giỏ chưa
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                // Nếu có rồi thì tăng số lượng lên 1
                item.Quantity++;
            }
            else
            {
                // Nếu chưa có thì truy vấn DB để lấy thông tin sản phẩm
                var product = _context.Products.Find(id);
                if (product != null)
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        ImageUrl = product.ImageUrl,
                        Quantity = 1
                    });
                }
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.Set("GioHang", cart);

            // Quay lại trang trước đó hoặc trang chủ
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var item = cart.FirstOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.Set("GioHang", cart);
            }
            return RedirectToAction("Index");
        }
    }
}