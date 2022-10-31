using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.PostgresqlMigrations.Migrations
{
    public partial class AddUrlsTable_ResorcesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileGroups_Authors_AuthorId",
                table: "FileGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Authors_AuthorId",
                table: "Files");

            migrationBuilder.DropTable(
                name: "CategoryFile");

            migrationBuilder.DropTable(
                name: "CategoryFileGroup");

            migrationBuilder.DropTable(
                name: "FileGroupTag");

            migrationBuilder.DropTable(
                name: "FileTag");

            migrationBuilder.DropIndex(
                name: "IX_Files_AuthorId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_FileGroups_AuthorId",
                table: "FileGroups");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ShortInfo",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "FileGroups");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FileGroups");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "FileGroups");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FileGroups");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FileGroups");

            migrationBuilder.DropColumn(
                name: "ShortInfo",
                table: "FileGroups");

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "Tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "Ratings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Links",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Files",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "Files",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "FileGroups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "FileGroups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UrlId",
                table: "Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "Categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShortInfo = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Urls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Urls_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ResourceId",
                table: "Tags",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ResourceId",
                table: "Ratings",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CategoryId",
                table: "Files",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_TagId",
                table: "Files",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_FileGroups_CategoryId",
                table: "FileGroups",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FileGroups_TagId",
                table: "FileGroups",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UrlId",
                table: "Comments",
                column: "UrlId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ResourceId",
                table: "Categories",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_AuthorId",
                table: "Resources",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Resources_ResourceId",
                table: "Categories",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Urls_UrlId",
                table: "Comments",
                column: "UrlId",
                principalTable: "Urls",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroups_Categories_CategoryId",
                table: "FileGroups",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroups_Resources_Id",
                table: "FileGroups",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroups_Tags_TagId",
                table: "FileGroups",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Categories_CategoryId",
                table: "Files",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Resources_Id",
                table: "Files",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Tags_TagId",
                table: "Files",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Resources_ResourceId",
                table: "Ratings",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Resources_ResourceId",
                table: "Tags",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Resources_ResourceId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Urls_UrlId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_FileGroups_Categories_CategoryId",
                table: "FileGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_FileGroups_Resources_Id",
                table: "FileGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_FileGroups_Tags_TagId",
                table: "FileGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Categories_CategoryId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Resources_Id",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Tags_TagId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Resources_ResourceId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Resources_ResourceId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "Urls");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ResourceId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_ResourceId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Files_CategoryId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_TagId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_FileGroups_CategoryId",
                table: "FileGroups");

            migrationBuilder.DropIndex(
                name: "IX_FileGroups_TagId",
                table: "FileGroups");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UrlId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ResourceId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "FileGroups");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "FileGroups");

            migrationBuilder.DropColumn(
                name: "UrlId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Links",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Files",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Files",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Files",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Files",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShortInfo",
                table: "Files",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Files",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "FileGroups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FileGroups",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FileGroups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FileGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FileGroups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShortInfo",
                table: "FileGroups",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoryFile",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    FilesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryFile", x => new { x.CategoriesId, x.FilesId });
                    table.ForeignKey(
                        name: "FK_CategoryFile_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryFile_Files_FilesId",
                        column: x => x.FilesId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryFileGroup",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileGroupsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryFileGroup", x => new { x.CategoriesId, x.FileGroupsId });
                    table.ForeignKey(
                        name: "FK_CategoryFileGroup_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryFileGroup_FileGroups_FileGroupsId",
                        column: x => x.FileGroupsId,
                        principalTable: "FileGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileGroupTag",
                columns: table => new
                {
                    FileGroupsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileGroupTag", x => new { x.FileGroupsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_FileGroupTag_FileGroups_FileGroupsId",
                        column: x => x.FileGroupsId,
                        principalTable: "FileGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileGroupTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileTag",
                columns: table => new
                {
                    FilesId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTag", x => new { x.FilesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_FileTag_Files_FilesId",
                        column: x => x.FilesId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_AuthorId",
                table: "Files",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_FileGroups_AuthorId",
                table: "FileGroups",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryFile_FilesId",
                table: "CategoryFile",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryFileGroup_FileGroupsId",
                table: "CategoryFileGroup",
                column: "FileGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_FileGroupTag_TagsId",
                table: "FileGroupTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_FileTag_TagsId",
                table: "FileTag",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroups_Authors_AuthorId",
                table: "FileGroups",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Authors_AuthorId",
                table: "Files",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
