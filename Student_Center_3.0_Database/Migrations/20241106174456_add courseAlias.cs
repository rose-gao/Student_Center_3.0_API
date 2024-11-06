using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    /// <inheritdoc />
    public partial class addcourseAlias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "courseAlias",
                table: "Courses",
                type: "nvarchar(60)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "courseAlias",
                table: "Courses");
        }
    }
}
