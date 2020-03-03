using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version308 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeparmentID",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeparmentID",
                table: "Tasks");
        }
    }
}
