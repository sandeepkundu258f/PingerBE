using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addtablePingLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PingTargets",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 20, 39, 33, 941, DateTimeKind.Utc).AddTicks(1210),
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PingTargets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PingLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PingTargetId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResponseTimeMs = table.Column<int>(type: "INTEGER", nullable: false),
                    IsSuccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PingLogs_PingTargets_PingTargetId",
                        column: x => x.PingTargetId,
                        principalTable: "PingTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PingTargets_Name",
                table: "PingTargets",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PingLogs_PingTargetId",
                table: "PingLogs",
                column: "PingTargetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PingLogs");

            migrationBuilder.DropIndex(
                name: "IX_PingTargets_Name",
                table: "PingTargets");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PingTargets");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PingTargets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2026, 6, 6, 20, 39, 33, 941, DateTimeKind.Utc).AddTicks(1210));
        }
    }
}
