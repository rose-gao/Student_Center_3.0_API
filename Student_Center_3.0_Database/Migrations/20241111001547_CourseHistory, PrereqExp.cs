using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class CourseHistoryPrereqExp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoursePrerequisites",
                columns: table => new
                {
                    course = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    prerequisiteExpression = table.Column<string>(type: "nvarchar(MAX)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequisites", x => x.course);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourseHistories",
                columns: table => new
                {
                    userNum = table.Column<int>(type: "int", nullable: false),
                    course = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseHistories", x => new { x.userNum, x.course });
                    table.ForeignKey(
                        name: "FK_StudentCourseHistories_Users_userNum",
                        column: x => x.userNum,
                        principalTable: "Users",
                        principalColumn: "userNum",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePrerequisites");

            migrationBuilder.DropTable(
                name: "StudentCourseHistories");
        }
    }
}
