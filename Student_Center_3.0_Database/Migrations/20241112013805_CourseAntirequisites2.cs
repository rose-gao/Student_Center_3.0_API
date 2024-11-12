using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class CourseAntirequisites2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseAntirequisite",
                table: "CourseAntirequisite");

            migrationBuilder.RenameTable(
                name: "CourseAntirequisite",
                newName: "CourseAntirequisites");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseAntirequisites",
                table: "CourseAntirequisites",
                columns: new[] { "course", "antirequisite" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseAntirequisites",
                table: "CourseAntirequisites");

            migrationBuilder.RenameTable(
                name: "CourseAntirequisites",
                newName: "CourseAntirequisite");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseAntirequisite",
                table: "CourseAntirequisite",
                columns: new[] { "course", "antirequisite" });
        }
    }
}
