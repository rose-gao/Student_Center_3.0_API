using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_API.Migrations
{
    /// <inheritdoc />
    public partial class Studentrefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logins_Students_studentNum",
                table: "Logins");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Logins",
                table: "Logins");

            migrationBuilder.RenameColumn(
                name: "studentNum",
                table: "Logins",
                newName: "userNum");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logins",
                table: "Logins",
                column: "userId");

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
                    socialInsuranceNum = table.Column<string>(type: "nvarchar(9)", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Logins_userNum",
                table: "Logins",
                column: "userNum",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Logins_Users_userNum",
                table: "Logins",
                column: "userNum",
                principalTable: "Users",
                principalColumn: "userNum",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logins_Users_userNum",
                table: "Logins");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Logins",
                table: "Logins");

            migrationBuilder.DropIndex(
                name: "IX_Logins_userNum",
                table: "Logins");

            migrationBuilder.RenameColumn(
                name: "userNum",
                table: "Logins",
                newName: "studentNum");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logins",
                table: "Logins",
                column: "studentNum");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    studentNum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    birthday = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    city = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    firstName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    middleName = table.Column<string>(type: "nvarchar(60)", nullable: true),
                    phoneNum = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    postalCode = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    province = table.Column<string>(type: "nvarchar(2)", nullable: false),
                    socialInsuranceNum = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    streetAddress = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.studentNum);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Logins_Students_studentNum",
                table: "Logins",
                column: "studentNum",
                principalTable: "Students",
                principalColumn: "studentNum",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
