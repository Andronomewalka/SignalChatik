using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalChatik.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AuthUserRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthUsers_AuthRoles_AuthUserRoleId",
                        column: x => x.AuthUserRoleId,
                        principalTable: "AuthRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChannelTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_ChannelTypes_ChannelTypeId",
                        column: x => x.ChannelTypeId,
                        principalTable: "ChannelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthRefreshTokens_AuthUsers_AuthUserId",
                        column: x => x.AuthUserId,
                        principalTable: "AuthUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChannelChannels",
                columns: table => new
                {
                    ConnectedChannelsId = table.Column<int>(type: "int", nullable: false),
                    ForChannelsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelChannels", x => new { x.ConnectedChannelsId, x.ForChannelsId });
                    table.ForeignKey(
                        name: "FK_ChannelChannels_Channels_ConnectedChannelsId",
                        column: x => x.ConnectedChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelChannels_Channels_ForChannelsId",
                        column: x => x.ForChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderChannelId = table.Column<int>(type: "int", nullable: true),
                    ReceiverChannelId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Channels_ReceiverChannelId",
                        column: x => x.ReceiverChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Channels_SenderChannelId",
                        column: x => x.SenderChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AuthRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "User" },
                    { 1, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "ChannelTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "User" },
                    { 1, "Room" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthRefreshTokens_AuthUserId",
                table: "AuthRefreshTokens",
                column: "AuthUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_AuthUserRoleId",
                table: "AuthUsers",
                column: "AuthUserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelChannels_ForChannelsId",
                table: "ChannelChannels",
                column: "ForChannelsId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ChannelTypeId",
                table: "Channels",
                column: "ChannelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverChannelId",
                table: "Messages",
                column: "ReceiverChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderChannelId",
                table: "Messages",
                column: "SenderChannelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthRefreshTokens");

            migrationBuilder.DropTable(
                name: "ChannelChannels");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "AuthUsers");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "AuthRoles");

            migrationBuilder.DropTable(
                name: "ChannelTypes");
        }
    }
}
