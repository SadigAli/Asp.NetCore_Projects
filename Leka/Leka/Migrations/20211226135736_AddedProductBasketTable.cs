using Microsoft.EntityFrameworkCore.Migrations;

namespace Leka.Migrations
{
    public partial class AddedProductBasketTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductBaskets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(nullable: true),
                    CategoryName = table.Column<string>(nullable: true),
                    SalePrice = table.Column<double>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    AppUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBaskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBaskets_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBaskets_AppUserId",
                table: "ProductBaskets",
                column: "AppUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductBaskets");
        }
    }
}
