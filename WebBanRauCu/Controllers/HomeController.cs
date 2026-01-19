using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBanRauCu.Models;

namespace WebBanRauCu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            // CHỐT CHẶN: Nếu đã đăng nhập và là Admin thì không cho xem trang chủ
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            var products = await productsQuery.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> FilterByCat(int id)
        {
            // CHỐT CHẶN: Áp dụng tương tự cho trang lọc theo danh mục
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id)
                .ToListAsync();

            return View("Index", products);
        }

        // Các hàm Privacy và Error giữ nguyên...
        public IActionResult Privacy() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}