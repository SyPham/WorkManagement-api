using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Version101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OCID",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OCID",
                table: "Users");
        }
    }
}
