using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalChatik.Migrations
{
    public partial class MessageDateAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeUtc",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTimeUtc",
                table: "Messages");
        }
    }
}
