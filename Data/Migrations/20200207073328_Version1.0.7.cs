using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Version107 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_OCs_OCID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_OCID",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_OCID",
                table: "Users",
                column: "OCID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_OCs_OCID",
                table: "Users",
                column: "OCID",
                principalTable: "OCs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
