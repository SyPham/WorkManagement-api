using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version208 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OCs_Tasks_TaskID1",
                table: "OCs");

            migrationBuilder.DropIndex(
                name: "IX_OCs_TaskID1",
                table: "OCs");

            migrationBuilder.DropColumn(
                name: "TaskID",
                table: "OCs");

            migrationBuilder.DropColumn(
                name: "TaskID1",
                table: "OCs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskID",
                table: "OCs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskID1",
                table: "OCs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OCs_TaskID1",
                table: "OCs",
                column: "TaskID1");

            migrationBuilder.AddForeignKey(
                name: "FK_OCs_Tasks_TaskID1",
                table: "OCs",
                column: "TaskID1",
                principalTable: "Tasks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
