using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramNews.Database.Migrations
{
    public partial class AddTgMessageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TgMessageId",
                table: "Posts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TgMessageId",
                table: "Posts");
        }
    }
}
