using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.MsSqlServerMigrations.Migrations
{
    public partial class Link_Model_Change_LastAccess_Type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)

        {
            migrationBuilder.DropColumn(
                name: "LastAccess",
                table: "Links");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccess",
                table: "Links",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAccess",
                table: "Links");

            migrationBuilder.AddColumn<int>(
                name: "LastAccess",
                table: "Links",
                type: "int",
                nullable: false);
        }
    }
}
