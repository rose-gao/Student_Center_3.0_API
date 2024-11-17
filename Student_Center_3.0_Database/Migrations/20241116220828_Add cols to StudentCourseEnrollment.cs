using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class AddcolstoStudentCourseEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "courseName",
                table: "StudentCourseEnrollments",
                type: "nvarchar(60)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "courseSuffix",
                table: "StudentCourseEnrollments",
                type: "nvarchar(3)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "courseWeight",
                table: "StudentCourseEnrollments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "courseName",
                table: "StudentCourseEnrollments");

            migrationBuilder.DropColumn(
                name: "courseSuffix",
                table: "StudentCourseEnrollments");

            migrationBuilder.DropColumn(
                name: "courseWeight",
                table: "StudentCourseEnrollments");
        }
    }
}
