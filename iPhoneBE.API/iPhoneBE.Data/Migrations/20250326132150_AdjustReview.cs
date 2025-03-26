using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPhoneBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdjustReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Reviews",
                newName: "ShipperComment");

            migrationBuilder.AddColumn<string>(
                name: "ProductComment",
                table: "Reviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductRating",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperRating",
                table: "Reviews",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductComment",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProductRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ShipperRating",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "ShipperComment",
                table: "Reviews",
                newName: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
