using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class CourseAntirequisites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseAntirequisite",
                columns: table => new
                {
                    course = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    antirequisite = table.Column<string>(type: "nvarchar(60)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAntirequisite", x => new { x.course, x.antirequisite });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseAntirequisite");
        }
    }
}
