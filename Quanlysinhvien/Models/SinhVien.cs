using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySinhVien.Models
{
    public class SinhVien
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lớp")]
        [StringLength(50)]
        [Display(Name = "Lớp")]
        public string Lop { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập điểm")]
        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        [Display(Name = "Điểm")]
        public double Diem { get; set; }


        // ==========================================
        // PHẦN UPLOAD HÌNH ẢNH SINH VIÊN
        // ==========================================

        // Lưu tên file ảnh vào Database
        // Ví dụ: 123abc.jpg
        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }


        // Nhận file ảnh người dùng chọn trên giao diện
        // Không tạo cột ImageFile trong Database
        [NotMapped]
        [Display(Name = "Chọn hình ảnh")]
        public IFormFile? ImageFile { get; set; }
    }
}