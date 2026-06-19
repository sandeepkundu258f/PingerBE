using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pinger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataRoleAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, false, "SuperAdmin" },
                    { 2, false, "Admin" },
                    { 3, false, "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsDeleted", "PasswordHash", "Username" },
                values: new object[] { 1, false, "$2a$12$SYJyDpVwm.1//dVZTKX.B.PdnIpDFqUk4LUsWdYwSqZBZk8LsHzwW", "sysadmin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "IsDeleted", "RoleId", "UserId" },
                values: new object[] { 1, false, 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
