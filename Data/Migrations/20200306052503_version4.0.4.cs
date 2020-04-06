using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version404 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDateMonthly",
                table: "Tasks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDateQuarterly",
                table: "Tasks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDateWeekly",
                table: "Tasks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDateYearly",
                table: "Tasks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Weekly",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Yearly",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Projects",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDateMonthly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DueDateQuarterly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DueDateWeekly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DueDateYearly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Weekly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Yearly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Projects");
        }
    }
}
