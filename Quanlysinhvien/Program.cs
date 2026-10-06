using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// Đăng ký MVC
// ==========================================
builder.Services.AddControllersWithViews();

// ==========================================
// Đăng ký Entity Framework Core
// ==========================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

// ==========================================
// Cấu hình môi trường
// ==========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ==========================================
// Middleware có sẵn
// ==========================================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ==========================================
// Middleware tự tạo
// ==========================================

app.UseMiddleware<RequestLoggingMiddleware>();

// ==========================================
// Route mặc định
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SinhVien}/{action=Index}/{id?}");

app.Run();