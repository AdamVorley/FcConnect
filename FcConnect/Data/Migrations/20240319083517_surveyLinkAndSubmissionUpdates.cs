using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class surveyLinkAndSubmissionUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionLinkId",
                table: "SurveyAnswer");

            migrationBuilder.RenameColumn(
                name: "SubmissionSurveyLinkId",
                table: "SurveySubmission",
                newName: "SurveyId");

            migrationBuilder.AddColumn<int>(
                name: "SurveySubmissionId",
                table: "SurveyAnswer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveySubmission_SurveyId",
                table: "SurveySubmission",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswer_SurveyId",
                table: "SurveyAnswer",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswer_SurveySubmissionId",
                table: "SurveyAnswer",
                column: "SurveySubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswer_SurveySubmission_SurveySubmissionId",
                table: "SurveyAnswer",
                column: "SurveySubmissionId",
                principalTable: "SurveySubmission",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswer_Survey_SurveyId",
                table: "SurveyAnswer",
                column: "SurveyId",
                principalTable: "Survey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveySubmission_Survey_SurveyId",
                table: "SurveySubmission",
                column: "SurveyId",
                principalTable: "Survey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAnswer_SurveySubmission_SurveySubmissionId",
                table: "SurveyAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAnswer_Survey_SurveyId",
                table: "SurveyAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveySubmission_Survey_SurveyId",
                table: "SurveySubmission");

            migrationBuilder.DropIndex(
                name: "IX_SurveySubmission_SurveyId",
                table: "SurveySubmission");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAnswer_SurveyId",
                table: "SurveyAnswer");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAnswer_SurveySubmissionId",
                table: "SurveyAnswer");

            migrationBuilder.DropColumn(
                name: "SurveySubmissionId",
                table: "SurveyAnswer");

            migrationBuilder.RenameColumn(
                name: "SurveyId",
                table: "SurveySubmission",
                newName: "SubmissionSurveyLinkId");

            migrationBuilder.AddColumn<int>(
                name: "SubmissionLinkId",
                table: "SurveyAnswer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
