using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Version108 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OCID",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskID",
                table: "OCs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskID1",
                table: "OCs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskOC",
                columns: table => new
                {
                    TaskID = table.Column<int>(nullable: false),
                    OCID = table.Column<int>(nullable: false),
                    ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskOC", x => new { x.OCID, x.TaskID });
                    table.ForeignKey(
                        name: "FK_TaskOC_OCs_OCID",
                        column: x => x.OCID,
                        principalTable: "OCs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskOC_Tasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "Tasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OCs_TaskID1",
                table: "OCs",
                column: "TaskID1");

            migrationBuilder.CreateIndex(
                name: "IX_TaskOC_TaskID",
                table: "TaskOC",
                column: "TaskID");

            migrationBuilder.AddForeignKey(
                name: "FK_OCs_Tasks_TaskID1",
                table: "OCs",
                column: "TaskID1",
                principalTable: "Tasks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OCs_Tasks_TaskID1",
                table: "OCs");

            migrationBuilder.DropTable(
                name: "TaskOC");

            migrationBuilder.DropIndex(
                name: "IX_OCs_TaskID1",
                table: "OCs");

            migrationBuilder.DropColumn(
                name: "OCID",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskID",
                table: "OCs");

            migrationBuilder.DropColumn(
                name: "TaskID1",
                table: "OCs");
        }
    }
}
