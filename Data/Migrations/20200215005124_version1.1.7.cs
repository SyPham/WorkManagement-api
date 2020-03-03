using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version117 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FinishedMainTask",
                table: "Tasks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishedMainTask",
                table: "Tasks");
        }
    }
}
