using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version115 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Managers_ProjectID",
                table: "Managers");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_ProjectID",
                table: "Managers",
                column: "ProjectID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Managers_ProjectID",
                table: "Managers");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_ProjectID",
                table: "Managers",
                column: "ProjectID");
        }
    }
}
