using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Center_3._0_API.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    studentNum = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(15)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.studentNum);
                    table.ForeignKey(
                        name: "FK_Logins_Students_studentNum",
                        column: x => x.studentNum,
                        principalTable: "Students",
                        principalColumn: "studentNum",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logins");
        }
    }
}
