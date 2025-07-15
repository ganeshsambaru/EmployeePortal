using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeePortal.web.Migrations
{
    /// <inheritdoc />
    public partial class AddAppUserIdToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AppUserId",
                table: "Employees",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AppUsers_AppUserId",
                table: "Employees",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AppUsers_AppUserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_AppUserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Employees");
        }
    }
}
