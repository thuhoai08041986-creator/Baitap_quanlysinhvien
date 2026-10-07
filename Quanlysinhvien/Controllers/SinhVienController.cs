using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        // Constructor
        public SinhVienController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        // =====================================================
        // 1. DANH SÁCH + TÌM KIẾM
        // =====================================================
        public async Task<IActionResult> Index(string searchString)
        {
            var sinhViens = _context.SinhViens.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                sinhViens = sinhViens.Where(sv =>
                    sv.HoTen.Contains(searchString) ||
                    sv.Email.Contains(searchString) ||
                    sv.Lop.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            return View(await sinhViens.ToListAsync());
        }


        // =====================================================
        // 2. CHI TIẾT SINH VIÊN
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .FirstOrDefaultAsync(sv => sv.Id == id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }


        // =====================================================
        // 3. CREATE - GET
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // =====================================================
        // 4. CREATE - POST
        // THÊM SINH VIÊN + UPLOAD ẢNH
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sinhVien)
        {
            // -----------------------------------------
            // Kiểm tra file ảnh
            // -----------------------------------------
            if (sinhVien.ImageFile != null)
            {
                string extension = Path.GetExtension(
                    sinhVien.ImageFile.FileName
                ).ToLowerInvariant();

                // Chỉ cho phép JPG, JPEG, PNG
                string[] allowedExtensions =
                {
                    ".jpg",
                    ".jpeg",
                    ".png"
                };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Chỉ được upload ảnh JPG hoặc PNG."
                    );
                }

                // Giới hạn dung lượng 5MB
                if (sinhVien.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Ảnh không được lớn hơn 5MB."
                    );
                }
            }

            // Nếu dữ liệu sai thì quay lại form
            if (!ModelState.IsValid)
            {
                return View(sinhVien);
            }


            // -----------------------------------------
            // Lưu ảnh lên server
            // -----------------------------------------
            if (sinhVien.ImageFile != null)
            {
                string extension = Path.GetExtension(
                    sinhVien.ImageFile.FileName
                ).ToLowerInvariant();

                // Tạo tên file mới tránh trùng
                string fileName =
                    Guid.NewGuid().ToString() + extension;

                // wwwroot/images/students
                string uploadFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "students"
                );

                // Nếu chưa có thư mục thì tự tạo
                Directory.CreateDirectory(uploadFolder);

                // Đường dẫn đầy đủ tới ảnh
                string filePath = Path.Combine(
                    uploadFolder,
                    fileName
                );

                // Lưu ảnh
                using (var stream =
                       new FileStream(filePath, FileMode.Create))
                {
                    await sinhVien.ImageFile.CopyToAsync(stream);
                }

                // Lưu tên ảnh vào đối tượng SinhVien
                sinhVien.HinhAnh = fileName;
            }


            // -----------------------------------------
            // Lưu sinh viên vào Database
            // -----------------------------------------
            _context.SinhViens.Add(sinhVien);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Thêm sinh viên thành công!";

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // 5. EDIT - GET
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien =
                await _context.SinhViens.FindAsync(id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }


        // =====================================================
        // 6. EDIT - POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            SinhVien sinhVien)
        {
            if (id != sinhVien.Id)
            {
                return NotFound();
            }


            // Lấy dữ liệu cũ
            var sinhVienCu =
                await _context.SinhViens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        sv => sv.Id == id);

            if (sinhVienCu == null)
            {
                return NotFound();
            }


            // -----------------------------------------
            // Nếu người dùng chọn ảnh mới
            // -----------------------------------------
            if (sinhVien.ImageFile != null)
            {
                string extension = Path.GetExtension(
                    sinhVien.ImageFile.FileName
                ).ToLowerInvariant();

                string[] allowedExtensions =
                {
                    ".jpg",
                    ".jpeg",
                    ".png"
                };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Chỉ được upload ảnh JPG hoặc PNG."
                    );
                }

                if (sinhVien.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Ảnh không được lớn hơn 5MB."
                    );
                }
            }


            if (!ModelState.IsValid)
            {
                // Giữ ảnh cũ khi validation lỗi
                sinhVien.HinhAnh =
                    sinhVienCu.HinhAnh;

                return View(sinhVien);
            }


            // -----------------------------------------
            // Upload ảnh mới
            // -----------------------------------------
            if (sinhVien.ImageFile != null)
            {
                string extension = Path.GetExtension(
                    sinhVien.ImageFile.FileName
                ).ToLowerInvariant();

                string fileName =
                    Guid.NewGuid().ToString() + extension;

                string uploadFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "students"
                );

                Directory.CreateDirectory(uploadFolder);

                string filePath = Path.Combine(
                    uploadFolder,
                    fileName
                );


                using (var stream =
                       new FileStream(filePath, FileMode.Create))
                {
                    await sinhVien.ImageFile.CopyToAsync(stream);
                }


                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(
                        sinhVienCu.HinhAnh))
                {
                    string oldImagePath =
                        Path.Combine(
                            uploadFolder,
                            sinhVienCu.HinhAnh
                        );

                    if (System.IO.File.Exists(
                            oldImagePath))
                    {
                        System.IO.File.Delete(
                            oldImagePath);
                    }
                }


                // Lưu tên ảnh mới
                sinhVien.HinhAnh = fileName;
            }
            else
            {
                // Không chọn ảnh mới
                // → giữ ảnh cũ
                sinhVien.HinhAnh =
                    sinhVienCu.HinhAnh;
            }


            try
            {
                _context.Update(sinhVien);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Cập nhật sinh viên thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SinhVienExists(sinhVien.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // 7. DELETE - GET
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien =
                await _context.SinhViens
                    .FirstOrDefaultAsync(
                        sv => sv.Id == id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }


        // =====================================================
        // 8. DELETE - POST
        // =====================================================
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var sinhVien =
                await _context.SinhViens.FindAsync(id);

            if (sinhVien == null)
            {
                return NotFound();
            }


            // -----------------------------------------
            // Xóa file ảnh trên server
            // -----------------------------------------
            if (!string.IsNullOrEmpty(
                    sinhVien.HinhAnh))
            {
                string imagePath =
                    Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        "students",
                        sinhVien.HinhAnh
                    );

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }


            // Xóa sinh viên khỏi Database
            _context.SinhViens.Remove(sinhVien);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Xóa sinh viên thành công!";

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // KIỂM TRA SINH VIÊN TỒN TẠI
        // =====================================================
        private bool SinhVienExists(int id)
        {
            return _context.SinhViens
                .Any(sv => sv.Id == id);
        }
    }
}