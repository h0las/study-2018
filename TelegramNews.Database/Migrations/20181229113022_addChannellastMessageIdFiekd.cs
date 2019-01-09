using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramNews.Database.Migrations
{
    public partial class addChannellastMessageIdFiekd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastMessageId",
                table: "Channels",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessageId",
                table: "Channels");
        }
    }
}
