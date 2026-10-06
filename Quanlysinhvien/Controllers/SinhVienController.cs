using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================
        // DANH SÁCH + TÌM KIẾM
        // ============================
        public async Task<IActionResult> Index(string searchString)
        {
            var danhSach = _context.SinhViens.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                danhSach = danhSach.Where(sv =>
                    sv.HoTen.Contains(searchString) ||
                    sv.Email.Contains(searchString) ||
                    sv.Lop.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            return View(await danhSach.ToListAsync());
        }


        // ============================
        //xem  CHI TIẾT
        // ============================
        public async Task<IActionResult> Details(int? id)
        //kiemtra id
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


        // Them sinh vien
        // GET: CREATE- hienthi
        // ============================
        public IActionResult Create()
        {
            return View();
        }


        // ============================
        // POST: CREATE-capnhat
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                _context.SinhViens.Add(sinhVien);//them sv vao db

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Thêm sinh viên thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(sinhVien);
        }


        // ============================
        // GET: EDIT
        // ============================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .FindAsync(id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }


        // ============================
        // POST: EDIT
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            SinhVien sinhVien)
        {
            if (id != sinhVien.Id)
                //ktr id co hop le
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.SinhViens.Update(sinhVien);//ID da cap nhat

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

            return View(sinhVien);
        }


        // ============================
        // GET: DELETE
        // ============================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //tim sv trong db truyen vao
            var sinhVien = await _context.SinhViens
                .FirstOrDefaultAsync(sv => sv.Id == id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }


        // ============================
        // POST: DELETE
        // ============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sinhVien =
                await _context.SinhViens.FindAsync(id);//tim trong bang db

            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Xóa sinh viên thành công!";
            }

            return RedirectToAction(nameof(Index));
        }


        // ============================
        // KIỂM TRA ID
        // ============================
        private bool SinhVienExists(int id)
        {
            return _context.SinhViens
                .Any(sv => sv.Id == id);
        }
    }
}