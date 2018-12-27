using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramNews.Database.Migrations
{
    public partial class AddedNewFieldsToEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "File",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Posts");
        }
    }
}
