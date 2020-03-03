using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version204 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelOC",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LevelOC",
                table: "Users");
        }
    }
}
