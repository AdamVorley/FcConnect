﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSurveyFrequency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SurveyFrequency",
                table: "SurveyUserLink",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurveyFrequency",
                table: "SurveyUserLink");
        }
    }
}
