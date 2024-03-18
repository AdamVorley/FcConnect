using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class userModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SurveyUserLink",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SurveySubmission",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Forename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyUserLink_UserId",
                table: "SurveyUserLink",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveySubmission_UserId",
                table: "SurveySubmission",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveySubmission_User_UserId",
                table: "SurveySubmission",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyUserLink_User_UserId",
                table: "SurveyUserLink",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveySubmission_User_UserId",
                table: "SurveySubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyUserLink_User_UserId",
                table: "SurveyUserLink");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_SurveyUserLink_UserId",
                table: "SurveyUserLink");

            migrationBuilder.DropIndex(
                name: "IX_SurveySubmission_UserId",
                table: "SurveySubmission");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SurveyUserLink",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SurveySubmission",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
