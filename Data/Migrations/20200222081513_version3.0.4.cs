using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version304 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Monthly",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quarterly",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Monthly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Quarterly",
                table: "Tasks");
        }
    }
}
