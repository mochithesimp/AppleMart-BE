using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPhoneBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdjustChatID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatID",
                table: "ChatMessages",
                newName: "ChatMessageID");

            migrationBuilder.AlterColumn<string>(
                name: "SenderID",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatMessageID",
                table: "ChatMessages",
                newName: "ChatID");

            migrationBuilder.AlterColumn<int>(
                name: "SenderID",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
