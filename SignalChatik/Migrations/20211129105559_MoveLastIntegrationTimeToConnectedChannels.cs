using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalChatik.Migrations
{
    public partial class MoveLastIntegrationTimeToConnectedChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastInteractionTime",
                table: "Channels");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastInteractionTime",
                table: "ConnectedChannels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastInteractionTime",
                table: "ConnectedChannels");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastInteractionTime",
                table: "Channels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
