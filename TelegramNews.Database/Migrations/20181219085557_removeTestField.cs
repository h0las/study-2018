using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramNews.Database.Migrations
{
    public partial class removeTestField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostCount",
                table: "Channels");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostCount",
                table: "Channels",
                nullable: false,
                defaultValue: 0);
        }
    }
}
