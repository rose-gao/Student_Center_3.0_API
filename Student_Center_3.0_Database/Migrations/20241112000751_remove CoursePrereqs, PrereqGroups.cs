using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class removeCoursePrereqsPrereqGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePrereqs");

            migrationBuilder.DropTable(
                name: "PrereqGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrereqGroups",
                columns: table => new
                {
                    groupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    creditRequirement = table.Column<double>(type: "float", nullable: true),
                    groupType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrereqGroups", x => x.groupId);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrereqs",
                columns: table => new
                {
                    courseName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    prerequisite = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    groupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrereqs", x => new { x.courseName, x.prerequisite });
                    table.ForeignKey(
                        name: "FK_CoursePrereqs_PrereqGroups_groupId",
                        column: x => x.groupId,
                        principalTable: "PrereqGroups",
                        principalColumn: "groupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrereqs_groupId",
                table: "CoursePrereqs",
                column: "groupId");
        }
    }
}
