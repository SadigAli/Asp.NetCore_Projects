using Microsoft.EntityFrameworkCore.Migrations;

namespace Leka.Migrations
{
    public partial class AddedColumnToProductBasketTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ProductBaskets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductBaskets");
        }
    }
}
