using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version506 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DateOfMonthly",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfMonthly",
                table: "Tasks");
        }
    }
}
