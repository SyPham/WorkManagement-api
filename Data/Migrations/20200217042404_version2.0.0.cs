using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version200 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EveryDay",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EveryDay",
                table: "Tasks");
        }
    }
}
