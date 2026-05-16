using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArtGalleryAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9510));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9510));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9510));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9520));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9520));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9520));

            migrationBuilder.UpdateData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9480));

            migrationBuilder.UpdateData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 13, 12, 24, 10, 817, DateTimeKind.Utc).AddTicks(9500));

            migrationBuilder.CreateIndex(
                name: "IX_Artists_Region",
                table: "Artists",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_Available_Type",
                table: "Artifacts",
                columns: new[] { "IsAvailable", "ArtTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_Title",
                table: "Artifacts",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_YearCreated",
                table: "Artifacts",
                column: "YearCreated");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Artists_Region",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artifacts_Available_Type",
                table: "Artifacts");

            migrationBuilder.DropIndex(
                name: "IX_Artifacts_Title",
                table: "Artifacts");

            migrationBuilder.DropIndex(
                name: "IX_Artifacts_YearCreated",
                table: "Artifacts");

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6640));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6640));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6650));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6650));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6650));

            migrationBuilder.UpdateData(
                table: "Artifacts",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6650));

            migrationBuilder.UpdateData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6610));

            migrationBuilder.UpdateData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6620));

            migrationBuilder.UpdateData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6620));

            migrationBuilder.UpdateData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6630));
        }
    }
}
