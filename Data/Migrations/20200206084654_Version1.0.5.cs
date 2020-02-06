using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Version105 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OCUsers",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    OCID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCUsers", x => new { x.UserID, x.OCID });
                    table.ForeignKey(
                        name: "FK_OCUsers_OCs_OCID",
                        column: x => x.OCID,
                        principalTable: "OCs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OCUsers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OCUsers_OCID",
                table: "OCUsers",
                column: "OCID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OCUsers");
        }
    }
}
