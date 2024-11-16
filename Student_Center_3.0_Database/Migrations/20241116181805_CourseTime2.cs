using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class CourseTime2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courseTimes_Courses_courseNum",
                table: "courseTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_courseTimes",
                table: "courseTimes");

            migrationBuilder.RenameTable(
                name: "courseTimes",
                newName: "CourseTimes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseTimes",
                table: "CourseTimes",
                columns: new[] { "courseNum", "weekday" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTimes_Courses_courseNum",
                table: "CourseTimes",
                column: "courseNum",
                principalTable: "Courses",
                principalColumn: "courseNum",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTimes_Courses_courseNum",
                table: "CourseTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseTimes",
                table: "CourseTimes");

            migrationBuilder.RenameTable(
                name: "CourseTimes",
                newName: "courseTimes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_courseTimes",
                table: "courseTimes",
                columns: new[] { "courseNum", "weekday" });

            migrationBuilder.AddForeignKey(
                name: "FK_courseTimes_Courses_courseNum",
                table: "courseTimes",
                column: "courseNum",
                principalTable: "Courses",
                principalColumn: "courseNum",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
