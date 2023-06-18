using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.Materials.DAL.SqlServer.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileGroups_Resources_Id",
                table: "FileGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Resources_Id",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Urls_Resources_Id",
                table: "Urls");

            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroups_Resources_Id",
                table: "FileGroups",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Resources_Id",
                table: "Files",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Urls_Resources_Id",
                table: "Urls",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileGroups_Resources_Id",
                table: "FileGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Resources_Id",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Urls_Resources_Id",
                table: "Urls");

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessCount = table.Column<int>(type: "int", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastAccess = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    SourceAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroups_Resources_Id",
                table: "FileGroups",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Resources_Id",
                table: "Files",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Urls_Resources_Id",
                table: "Urls",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id");
        }
    }
}
