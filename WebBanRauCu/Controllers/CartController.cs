using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanRauCu.Models;
using WebBanRauCu.Helpers;

namespace WebBanRauCu.Controllers
{
    [Authorize] // YÊU CẦU: Bắt buộc đăng nhập mới được vào
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
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

            HttpContext.Session.Set("GioHang", cart);
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

        // ==========================================
        // KHU VỰC THANH TOÁN (ĐÃ SỬA LẠI LOGIC VALIDATION)
        // ==========================================

        // BƯỚC 1: Hiển thị form xác nhận thông tin (GET)
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(item => item.Total);

            // Lấy thông tin user hiện tại để điền sẵn vào form
            var user = await _userManager.GetUserAsync(User);
            var order = new Order
            {
                CustomerName = user.Name ?? user.UserName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return View(order);
        }

        // BƯỚC 2: Xử lý khi người dùng nhấn nút "Đặt hàng" (POST)
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            if (cart == null || cart.Count == 0) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);

            // --- ĐOẠN CODE QUAN TRỌNG VỪA THÊM VÀO ---
            // Kiểm tra tính hợp lệ của dữ liệu (Rỗng, SĐT sai định dạng...)
            if (!ModelState.IsValid)
            {
                // Nếu sai: Nạp lại thông tin giỏ hàng vào ViewBag để hiển thị lại View
                ViewBag.Cart = cart;
                ViewBag.Total = cart.Sum(item => item.Total);

                // Trả về View cũ kèm thông báo lỗi
                return View(order);
            }
            // ------------------------------------------

            // Gán thêm các dữ liệu hệ thống tự động
            order.UserId = user.Id;
            order.OrderDate = DateTime.Now;
            order.TotalAmount = (decimal)cart.Sum(i => i.Total);
            order.Status = 1; // 1: Đang xử lý

            // Lưu thông tin đơn hàng chính
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Lưu chi tiết đơn hàng
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng sau khi đặt thành công
            HttpContext.Session.Remove("GioHang");

            return RedirectToAction("MyOrders");
        }

        // Xem lịch sử đơn hàng
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders
                                       .Where(o => o.UserId == user.Id)
                                       .OrderByDescending(o => o.OrderDate)
                                       .ToListAsync();
            return View(orders);
        }
    }
}