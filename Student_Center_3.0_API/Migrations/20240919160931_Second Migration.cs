using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_API.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "middleName",
                table: "Students",
                type: "nvarchar(60)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "middleName",
                table: "Students",
                type: "nvarchar(60)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldNullable: true);
        }
    }
}
