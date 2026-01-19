using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebBanRauCu.Models; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Kết nối SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Cấu hình Identity (Sử dụng AddIdentity thay vì AddDefaultIdentity)
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false; 
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Đăng ký Razor Pages (Identity mặc định cần cái này)
builder.Services.AddRazorPages();

// 3. Đăng ký dịch vụ Session (QUAN TRỌNG)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// QUAN TRỌNG: Thêm dòng này để load được ảnh sản phẩm bạn upload vào wwwroot
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 4. Kích hoạt Session (QUAN TRỌNG: Phải đặt trước MapControllerRoute)
app.UseSession();

app.MapStaticAssets();

// Route cho Admin (Đặt TRÊN route mặc định)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Run();

app.MapRazorPages();
app.Run();