using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quanlysinhvien.Migrations
{
    /// <inheritdoc />
    public partial class AddHinhAnhToSinhVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "SinhViens");
        }
    }
}
