using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class changesuffixtodatesinStudentCourseEnrollments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "courseSuffix",
                table: "StudentCourseEnrollments");

            migrationBuilder.AddColumn<DateTime>(
                name: "endDate",
                table: "StudentCourseEnrollments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "startDate",
                table: "StudentCourseEnrollments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endDate",
                table: "StudentCourseEnrollments");

            migrationBuilder.DropColumn(
                name: "startDate",
                table: "StudentCourseEnrollments");

            migrationBuilder.AddColumn<string>(
                name: "courseSuffix",
                table: "StudentCourseEnrollments",
                type: "nvarchar(3)",
                nullable: false,
                defaultValue: "");
        }
    }
}
