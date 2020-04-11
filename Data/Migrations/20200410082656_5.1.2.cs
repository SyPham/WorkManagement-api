using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _512 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModifyDateTime",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifyDateTime",
                table: "Tasks");
        }
    }
}
