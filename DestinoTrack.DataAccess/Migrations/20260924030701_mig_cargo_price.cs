using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DestinoTrack.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig_cargo_price : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CargoPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteScope = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PricePerDesi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransitDays = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoPrices_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CargoTypeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CargoType = table.Column<int>(type: "int", nullable: false),
                    Multiplier = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TransitDaysDelta = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoTypeRates", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CargoTypeRates",
                columns: new[] { "Id", "CargoType", "CreatedDate", "IsDeleted", "Multiplier", "TransitDaysDelta", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("cc000000-0000-0000-0000-000000000001"), 1, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, 1.00m, 0, null },
                    { new Guid("cc000000-0000-0000-0000-000000000002"), 2, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, 1.50m, -1, null },
                    { new Guid("cc000000-0000-0000-0000-000000000003"), 3, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, 1.30m, 0, null },
                    { new Guid("cc000000-0000-0000-0000-000000000004"), 4, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, 1.40m, 0, null },
                    { new Guid("cc000000-0000-0000-0000-000000000005"), 5, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, 0.80m, 0, null },
                    { new Guid("cc000000-0000-0000-0000-000000000006"), 6, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, 1.80m, 0, null },
                    { new Guid("cc000000-0000-0000-0000-000000000007"), 7, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, 1.60m, 0, null },
                    { new Guid("cc000000-0000-0000-0000-000000000008"), 8, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, 1.70m, 1, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CargoPrices_CountryId_RouteScope",
                table: "CargoPrices",
                columns: new[] { "CountryId", "RouteScope" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CargoTypeRates_CargoType",
                table: "CargoTypeRates",
                column: "CargoType",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargoPrices");

            migrationBuilder.DropTable(
                name: "CargoTypeRates");
        }
    }
}
