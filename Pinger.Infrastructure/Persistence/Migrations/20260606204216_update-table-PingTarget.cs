using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updatetablePingTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PingTargets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2026, 6, 6, 20, 39, 33, 941, DateTimeKind.Utc).AddTicks(1210));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PingTargets",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 20, 39, 33, 941, DateTimeKind.Utc).AddTicks(1210),
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }
    }
}
