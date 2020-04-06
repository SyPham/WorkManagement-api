using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version405 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weekly",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "Daily",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Daily",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "Weekly",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
