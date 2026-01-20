using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanRauCu.Models;

namespace WebBanRauCu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.OrderBy(c => c.DisplayOrder).ToListAsync());
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var isDuplicateName = await _context.Categories.AnyAsync(c => c.Name == category.Name);
                if (isDuplicateName)
                {
                    ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại!");
                    return View(category);
                }

                var isDuplicateOrder = await _context.Categories.AnyAsync(c => c.DisplayOrder == category.DisplayOrder);
                if (isDuplicateOrder)
                {
                    ModelState.AddModelError("DisplayOrder", $"Thứ tự số {category.DisplayOrder} đã được sử dụng. Vui lòng chọn số khác!");
                    return View(category);
                }

                if (category.DisplayOrder < 0)
                {
                    ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị không được là số âm.");
                    return View(category);
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var isDuplicateName = await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != id);
                if (isDuplicateName)
                {
                    ModelState.AddModelError("Name", "Tên danh mục này đã được sử dụng!");
                    return View(category);
                }

                var isDuplicateOrder = await _context.Categories.AnyAsync(c => c.DisplayOrder == category.DisplayOrder && c.Id != id);
                if (isDuplicateOrder)
                {
                    ModelState.AddModelError("DisplayOrder", $"Thứ tự số {category.DisplayOrder} đang bị trùng với danh mục khác!");
                    return View(category);
                }

                if (category.DisplayOrder < 0)
                {
                    ModelState.AddModelError("DisplayOrder", "Thứ tự không được âm.");
                    return View(category);
                }

                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}