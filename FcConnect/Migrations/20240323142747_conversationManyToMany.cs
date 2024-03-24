using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcConnect.Migrations
{
    /// <inheritdoc />
    public partial class conversationManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Conversation_ConversationId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ConversationId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "User");

            migrationBuilder.CreateTable(
                name: "ConversationUser",
                columns: table => new
                {
                    ConversationsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationUser", x => new { x.ConversationsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ConversationUser_Conversation_ConversationsId",
                        column: x => x.ConversationsId,
                        principalTable: "Conversation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationUser_UsersId",
                table: "ConversationUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationUser");

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ConversationId",
                table: "User",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Conversation_ConversationId",
                table: "User",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id");
        }
    }
}
