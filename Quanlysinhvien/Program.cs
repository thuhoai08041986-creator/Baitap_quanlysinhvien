using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký MVC
builder.Services.AddControllersWithViews();

// Đăng ký Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

// Xử lý lỗi khi chạy production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS
app.UseHttpsRedirection();

// Cho phép sử dụng CSS, JS, hình ảnh
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SinhVien}/{action=Index}/{id?}");

app.Run();