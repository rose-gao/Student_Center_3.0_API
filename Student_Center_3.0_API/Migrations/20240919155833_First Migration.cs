using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_API.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    studentNum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    middleName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    birthday = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    socialInsuranceNum = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    phoneNum = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    streetAddress = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    city = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    province = table.Column<string>(type: "nvarchar(2)", nullable: false),
                    postalCode = table.Column<string>(type: "nvarchar(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.studentNum);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
