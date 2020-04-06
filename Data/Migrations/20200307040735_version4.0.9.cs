using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class version409 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Daily",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Monthly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Quarterly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Weekly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Yearly",
                table: "Tasks");

            migrationBuilder.AlterColumn<string>(
                name: "DueDateYearly",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DueDateQuarterly",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DueDateDaily",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DueDateWeekly",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecificDate",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDateDaily",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DueDateWeekly",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SpecificDate",
                table: "Tasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDateYearly",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDateQuarterly",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Daily",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Monthly",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quarterly",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Weekly",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Yearly",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
