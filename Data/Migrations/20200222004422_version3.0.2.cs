using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version302 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "ProjectID",
                table: "Rooms",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "RoomID",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
