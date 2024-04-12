using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcConnect.Migrations
{
    /// <inheritdoc />
    public partial class submissionUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewerId",
                table: "SurveySubmission",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "SurveySubmission");
        }
    }
}
