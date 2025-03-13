using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPhoneBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class addNewRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogViews_AspNetUsers_UserId1",
                table: "UserBlogViews");

            migrationBuilder.DropIndex(
                name: "IX_UserBlogViews_UserId1",
                table: "UserBlogViews");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserBlogViews");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserBlogViews",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f7c2d1b8-1234-4e56-7890-abcdef123456", null, "Shipper", "SHIPPER" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBlogViews_UserId",
                table: "UserBlogViews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBlogViews_AspNetUsers_UserId",
                table: "UserBlogViews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogViews_AspNetUsers_UserId",
                table: "UserBlogViews");

            migrationBuilder.DropIndex(
                name: "IX_UserBlogViews_UserId",
                table: "UserBlogViews");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f7c2d1b8-1234-4e56-7890-abcdef123456");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserBlogViews",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserBlogViews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBlogViews_UserId1",
                table: "UserBlogViews",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBlogViews_AspNetUsers_UserId1",
                table: "UserBlogViews",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
