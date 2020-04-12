using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version511 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModifyDateTime",
                table: "Histories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifyDateTime",
                table: "Histories");
        }
    }
}
