using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcConnect.Migrations
{
    /// <inheritdoc />
    public partial class userStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserStatusId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserStatusId",
                table: "User");
        }
    }
}
