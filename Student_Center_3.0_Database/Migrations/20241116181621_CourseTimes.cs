using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class CourseTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "courseTime",
                table: "Courses");

            migrationBuilder.CreateTable(
                name: "courseTimes",
                columns: table => new
                {
                    courseNum = table.Column<int>(type: "int", nullable: false),
                    weekday = table.Column<int>(type: "int", nullable: false),
                    startTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    endTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courseTimes", x => new { x.courseNum, x.weekday });
                    table.ForeignKey(
                        name: "FK_courseTimes_Courses_courseNum",
                        column: x => x.courseNum,
                        principalTable: "Courses",
                        principalColumn: "courseNum",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourseEnrollments",
                columns: table => new
                {
                    userNum = table.Column<int>(type: "int", nullable: false),
                    courseNum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseEnrollments", x => new { x.userNum, x.courseNum });
                    table.ForeignKey(
                        name: "FK_StudentCourseEnrollments_Courses_courseNum",
                        column: x => x.courseNum,
                        principalTable: "Courses",
                        principalColumn: "courseNum",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourseEnrollments_Users_userNum",
                        column: x => x.userNum,
                        principalTable: "Users",
                        principalColumn: "userNum",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseEnrollments_courseNum",
                table: "StudentCourseEnrollments",
                column: "courseNum");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "courseTimes");

            migrationBuilder.DropTable(
                name: "StudentCourseEnrollments");

            migrationBuilder.AddColumn<string>(
                name: "courseTime",
                table: "Courses",
                type: "nvarchar(150)",
                nullable: false,
                defaultValue: "");
        }
    }
}
