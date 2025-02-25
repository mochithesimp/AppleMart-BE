using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPhoneBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdjustDBV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSpecifications");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ProductItems");

            migrationBuilder.AddColumn<int>(
                name: "DisplayIndex",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayIndex",
                table: "ProductItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayIndex",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    AttributeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.AttributeID);
                    table.ForeignKey(
                        name: "FK_Attributes_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductItemAttributes",
                columns: table => new
                {
                    ProductItemAttributeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductItemID = table.Column<int>(type: "int", nullable: false),
                    AttributeID = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductItemAttributes", x => x.ProductItemAttributeID);
                    table.ForeignKey(
                        name: "FK_ProductItemAttributes_Attributes_AttributeID",
                        column: x => x.AttributeID,
                        principalTable: "Attributes",
                        principalColumn: "AttributeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductItemAttributes_ProductItems_ProductItemID",
                        column: x => x.ProductItemID,
                        principalTable: "ProductItems",
                        principalColumn: "ProductItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_CategoryID",
                table: "Attributes",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductItemAttributes_AttributeID",
                table: "ProductItemAttributes",
                column: "AttributeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductItemAttributes_ProductItemID",
                table: "ProductItemAttributes",
                column: "ProductItemID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductItemAttributes");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropColumn(
                name: "DisplayIndex",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DisplayIndex",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "DisplayIndex",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ProductItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductSpecifications",
                columns: table => new
                {
                    ProductSpecificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductItemID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SpecificationName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SpecificationValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecifications", x => x.ProductSpecificationID);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_ProductItems_ProductItemID",
                        column: x => x.ProductItemID,
                        principalTable: "ProductItems",
                        principalColumn: "ProductItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_ProductItemID",
                table: "ProductSpecifications",
                column: "ProductItemID");
        }
    }
}
