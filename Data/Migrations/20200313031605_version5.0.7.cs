using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version507 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Histories");

            migrationBuilder.AddColumn<string>(
                name: "Deadline",
                table: "Histories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Histories");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Histories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
