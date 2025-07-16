using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeePortal.web.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "AppUsers",
                newName: "Password");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "AppUsers",
                newName: "PasswordHash");
        }
    }
}
