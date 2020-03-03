using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version205 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isLeader",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isLeader",
                table: "Users");
        }
    }
}
