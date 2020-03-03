using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version309 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeparmentID",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentID",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "DeparmentID",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
