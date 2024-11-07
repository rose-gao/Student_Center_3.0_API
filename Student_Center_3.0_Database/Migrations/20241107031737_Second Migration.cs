using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursePrereqs_Courses_courseNum",
                table: "CoursePrereqs");

            migrationBuilder.DropIndex(
                name: "IX_CoursePrereqs_courseNum",
                table: "CoursePrereqs");

            migrationBuilder.DropColumn(
                name: "courseNum",
                table: "CoursePrereqs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "courseNum",
                table: "CoursePrereqs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrereqs_courseNum",
                table: "CoursePrereqs",
                column: "courseNum");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePrereqs_Courses_courseNum",
                table: "CoursePrereqs",
                column: "courseNum",
                principalTable: "Courses",
                principalColumn: "courseNum");
        }
    }
}
