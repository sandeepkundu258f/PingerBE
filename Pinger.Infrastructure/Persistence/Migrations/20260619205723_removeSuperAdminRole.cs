using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removeSuperAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "User");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "SuperAdmin");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[] { 1, false, "Admin" });
        }
    }
}
