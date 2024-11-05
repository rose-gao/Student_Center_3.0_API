using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_API.Migrations
{
    /// <inheritdoc />
    public partial class Coursetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    courseNum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    courseDesc = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    extraInformation = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    prerequisites = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    antirequisites = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    courseWeight = table.Column<int>(type: "int", nullable: false),
                    courseSemester = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    courseDay = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    courseTime = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    instructor = table.Column<string>(type: "nvarchar(60)", nullable: true),
                    room = table.Column<string>(type: "nvarchar(60)", nullable: true),
                    numEnrolled = table.Column<int>(type: "int", nullable: false),
                    totalSeats = table.Column<int>(type: "int", nullable: false),
                    isLab = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.courseNum);
                });

            migrationBuilder.CreateTable(
                name: "PrereqGroups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreditRequirement = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrereqGroups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrereqs",
                columns: table => new
                {
                    CourseNum = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteNum = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrereqs", x => new { x.CourseNum, x.PrerequisiteNum, x.GroupId });
                    table.ForeignKey(
                        name: "FK_CoursePrereqs_Courses_CourseNum",
                        column: x => x.CourseNum,
                        principalTable: "Courses",
                        principalColumn: "courseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoursePrereqs_Courses_PrerequisiteNum",
                        column: x => x.PrerequisiteNum,
                        principalTable: "Courses",
                        principalColumn: "courseNum",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursePrereqs_PrereqGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "PrereqGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrereqs_GroupId",
                table: "CoursePrereqs",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrereqs_PrerequisiteNum",
                table: "CoursePrereqs",
                column: "PrerequisiteNum");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePrereqs");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "PrereqGroups");
        }
    }
}
