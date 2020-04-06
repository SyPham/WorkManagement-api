using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version508 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskID",
                table: "Tutorials",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskID",
                table: "Tutorials");
        }
    }
}
