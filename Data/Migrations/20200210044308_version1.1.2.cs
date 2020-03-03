using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version112 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromWhoID",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FromWhos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    TaskID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FromWhos", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FromWhos");

            migrationBuilder.DropColumn(
                name: "FromWhoID",
                table: "Tasks");
        }
    }
}
