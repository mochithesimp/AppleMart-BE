using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPhoneBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateAstribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Vouchers",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsRemoved",
                table: "Notifications",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Blogs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "AspNetUsers",
                newName: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Vouchers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Notifications",
                newName: "IsRemoved");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Blogs",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "AspNetUsers",
                newName: "IsActive");
        }
    }
}
