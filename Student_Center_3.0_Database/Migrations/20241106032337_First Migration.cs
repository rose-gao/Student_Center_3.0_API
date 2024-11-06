using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
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
                    courseSuffix = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    courseDesc = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    extraInformation = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    prerequisites = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    antirequisites = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
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
                    table.UniqueConstraint("AK_Courses_courseName", x => x.courseName);
                });

            migrationBuilder.CreateTable(
                name: "PrereqGroups",
                columns: table => new
                {
                    groupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    groupType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    creditRequirement = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrereqGroups", x => x.groupId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userNum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    middleName = table.Column<string>(type: "nvarchar(60)", nullable: true),
                    lastName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    birthday = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    socialInsuranceNum = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    phoneNum = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    streetAddress = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    city = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    province = table.Column<string>(type: "nvarchar(2)", nullable: false),
                    postalCode = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userNum);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrereqs",
                columns: table => new
                {
                    courseName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    prerequisite = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    groupId = table.Column<int>(type: "int", nullable: false),
                    course = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrereqs", x => new { x.courseName, x.prerequisite });
                    table.ForeignKey(
                        name: "FK_CoursePrereqs_Courses_courseName",
                        column: x => x.courseName,
                        principalTable: "Courses",
                        principalColumn: "courseName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoursePrereqs_PrereqGroups_groupId",
                        column: x => x.groupId,
                        principalTable: "PrereqGroups",
                        principalColumn: "groupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    userId = table.Column<string>(type: "nvarchar(15)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    userNum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.userId);
                    table.ForeignKey(
                        name: "FK_Logins_Users_userNum",
                        column: x => x.userNum,
                        principalTable: "Users",
                        principalColumn: "userNum",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrereqs_groupId",
                table: "CoursePrereqs",
                column: "groupId");

            migrationBuilder.CreateIndex(
                name: "IX_Logins_userNum",
                table: "Logins",
                column: "userNum",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePrereqs");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "PrereqGroups");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
