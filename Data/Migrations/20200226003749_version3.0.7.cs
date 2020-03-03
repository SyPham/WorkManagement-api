using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version307 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ProjectID",
                table: "Rooms",
                column: "ProjectID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Projects_ProjectID",
                table: "Rooms",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Projects_ProjectID",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ProjectID",
                table: "Rooms");
        }
    }
}
