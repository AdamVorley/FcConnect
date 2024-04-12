using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcConnect.Migrations
{
    /// <inheritdoc />
    public partial class surveyUserLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedByUserId",
                table: "SurveyUserLink",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedByUserId",
                table: "SurveyUserLink");
        }
    }
}
