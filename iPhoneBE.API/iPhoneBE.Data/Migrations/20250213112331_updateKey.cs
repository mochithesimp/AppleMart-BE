using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPhoneBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserBlogViewId",
                table: "UserBlogViews",
                newName: "UserBlogViewID");

            migrationBuilder.RenameColumn(
                name: "NotificationId",
                table: "Notifications",
                newName: "NotificationID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ChatParticipants",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "BlogId",
                table: "Blogs",
                newName: "BlogID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserBlogViewID",
                table: "UserBlogViews",
                newName: "UserBlogViewId");

            migrationBuilder.RenameColumn(
                name: "NotificationID",
                table: "Notifications",
                newName: "NotificationId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ChatParticipants",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BlogID",
                table: "Blogs",
                newName: "BlogId");
        }
    }
}
