using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanRauCu.Models;
using WebBanRauCu.ViewModels;

namespace WebBanRauCu.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        // ===== LOGIN =====
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username!);
                    var roles = await _userManager.GetRolesAsync(user!);

                    // --- SỬA ĐOẠN NÀY ---
                    if (roles.Contains("Admin"))
                    {
                        // Chuyển hướng đến Dashboard thay vì Product
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    // --------------------

                    return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Home") : LocalRedirect(returnUrl);
                }
                ModelState.AddModelError("", "Đăng nhập không thành công.");
            }
            return View(model);
        }
        // ===== REGISTER =====
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = new AppUser
            {
                Name = model.Name,
                UserName = model.Email,
                Email = model.Email,
                Address = model.Address
            };
            var result = await _userManager.CreateAsync(user,

            model.Password!);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToLocal(returnUrl);
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            return !string.IsNullOrEmpty(returnUrl) &&

            Url.IsLocalUrl(returnUrl)

            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home");

        }
    }
}
