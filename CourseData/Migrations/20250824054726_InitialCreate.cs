using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseRegisterApp.CourseData.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CourseStart = table.Column<DateOnly>(type: "date", nullable: false),
                    CourseEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    MaxStudents = table.Column<int>(type: "int", nullable: false),
                    Lecturer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Course_User",
                        column: x => x.Lecturer,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "CourseUser",
                columns: table => new
                {
                    CourseUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseUser", x => x.CourseUserId);
                    table.ForeignKey(
                        name: "FK_CourseUser_Course",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "CourseId");
                    table.ForeignKey(
                        name: "FK_CourseUser_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Lecturer",
                table: "Course",
                column: "Lecturer");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-20250823-215620",
                table: "Course",
                column: "CourseName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser",
                table: "CourseUser",
                columns: new[] { "CourseId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser_UserId",
                table: "CourseUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseUser");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
