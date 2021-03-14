using Microsoft.EntityFrameworkCore.Migrations;

namespace MyChat.Migrations
{
    public partial class JoinId2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JoinId",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinId",
                table: "Chats");
        }
    }
}
