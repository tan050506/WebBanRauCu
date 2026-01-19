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

        // Tên Session Key dùng chung cho toàn bộ Controller
        private const string CART_KEY = "GioHang";

        public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Xem giỏ hàng
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            return View(cart);
        }

        // Thêm vào giỏ hàng
        public IActionResult AddToCart(int id)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
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

            HttpContext.Session.Set(CART_KEY, cart);
            return RedirectToAction("Index");
        }

        // Tăng/Giảm số lượng sản phẩm (Đã sửa lỗi Session Key)
        public IActionResult UpdateQuantity(int id, int amount)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                item.Quantity += amount;
                if (item.Quantity <= 0)
                {
                    cart.Remove(item); // Nếu giảm xuống 0 hoặc âm thì xóa khỏi giỏ
                }
            }

            HttpContext.Session.Set(CART_KEY, cart);
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY);
            if (cart != null)
            {
                var item = cart.FirstOrDefault(p => p.ProductId == id);
                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.Set(CART_KEY, cart);
                }
            }
            return RedirectToAction("Index");
        }

        // ==========================================
        // KHU VỰC THANH TOÁN
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY);
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(item => item.Total);

            var user = await _userManager.GetUserAsync(User);
            var order = new Order
            {
                CustomerName = user.Name ?? user.UserName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY);
            if (cart == null || cart.Count == 0) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                ViewBag.Cart = cart;
                ViewBag.Total = cart.Sum(item => item.Total);
                return View(order);
            }

            order.UserId = user.Id;
            order.OrderDate = DateTime.Now;
            order.TotalAmount = (decimal)cart.Sum(i => i.Total);
            order.Status = 1; 

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

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
            HttpContext.Session.Remove(CART_KEY);

            return RedirectToAction("MyOrders");
        }

        // Xem lịch sử đơn hàng
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            var orders = await _context.Orders
                                       .Where(o => o.UserId == user.Id)
                                       .OrderByDescending(o => o.OrderDate)
                                       .ToListAsync();
            return View(orders);
        }
    }
}