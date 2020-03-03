using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version303 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "Room",
                table: "Projects",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "ProjectID",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
