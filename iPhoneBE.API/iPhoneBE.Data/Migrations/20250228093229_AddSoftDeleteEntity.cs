using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPhoneBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChatRooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChatParticipants",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChatParticipants");
        }
    }
}
