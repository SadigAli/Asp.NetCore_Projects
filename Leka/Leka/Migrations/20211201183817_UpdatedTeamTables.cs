using Microsoft.EntityFrameworkCore.Migrations;

namespace Leka.Migrations
{
    public partial class UpdatedTeamTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_SosialIcons_SosialIconId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_SosialIconId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "SosialIconId",
                table: "Teams");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "SosialIcons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SosialIcons_TeamId",
                table: "SosialIcons",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_SosialIcons_Teams_TeamId",
                table: "SosialIcons",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SosialIcons_Teams_TeamId",
                table: "SosialIcons");

            migrationBuilder.DropIndex(
                name: "IX_SosialIcons_TeamId",
                table: "SosialIcons");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "SosialIcons");

            migrationBuilder.AddColumn<int>(
                name: "SosialIconId",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SosialIconId",
                table: "Teams",
                column: "SosialIconId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_SosialIcons_SosialIconId",
                table: "Teams",
                column: "SosialIconId",
                principalTable: "SosialIcons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
