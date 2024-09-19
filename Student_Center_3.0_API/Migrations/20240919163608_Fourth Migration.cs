using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_API.Migrations
{
    /// <inheritdoc />
    public partial class FourthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phoneNum",
                table: "Students",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phoneNum",
                table: "Students",
                type: "nvarchar(9)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");
        }
    }
}
