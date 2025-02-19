using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace iPhoneBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class addRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b1a290ef-4467-4339-9931-218d8aeb46c0", null, "Staff", "STAFF" },
                    { "e450c7fa-cb9b-40b0-b5f2-7ad083eefd63", null, "Customer", "CUSTOMER" },
                    { "e5df0673-2801-41c1-924c-8a84ee3aaa70", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1a290ef-4467-4339-9931-218d8aeb46c0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e450c7fa-cb9b-40b0-b5f2-7ad083eefd63");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5df0673-2801-41c1-924c-8a84ee3aaa70");
        }
    }
}
