using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanRauCu.Models;

namespace WebBanRauCu.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public MenuLoaiViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(items);
        }
    }
}