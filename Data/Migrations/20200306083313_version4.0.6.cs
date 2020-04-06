using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version406 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDateWeekly",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "EveryDay",
                table: "Tasks",
                newName: "Weekly");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weekly",
                table: "Tasks",
                newName: "EveryDay");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDateWeekly",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
