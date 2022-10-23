using Microsoft.EntityFrameworkCore.Migrations;

namespace ResteurantApi.Migrations
{
    public partial class autoryzacjaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Resteurants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resteurants_CreatedById",
                table: "Resteurants",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Resteurants_User_CreatedById",
                table: "Resteurants",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resteurants_User_CreatedById",
                table: "Resteurants");

            migrationBuilder.DropIndex(
                name: "IX_Resteurants_CreatedById",
                table: "Resteurants");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Resteurants");
        }
    }
}
