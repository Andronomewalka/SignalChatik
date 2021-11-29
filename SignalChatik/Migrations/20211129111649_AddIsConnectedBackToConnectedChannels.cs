using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalChatik.Migrations
{
    public partial class AddIsConnectedBackToConnectedChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConnectedBack",
                table: "ConnectedChannels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConnectedBack",
                table: "ConnectedChannels");
        }
    }
}
