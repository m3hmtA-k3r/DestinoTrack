using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DestinoTrack.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig_vehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Plate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_VehicleId",
                table: "Employees",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_BranchId",
                table: "Vehicles",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Plate",
                table: "Vehicles",
                column: "Plate",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Vehicles_VehicleId",
                table: "Employees",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // --- Veri taşıma (D101): mevcut kuryelerin plakaları araç havuzuna çevrilir ---
            // Tekil indeks plakanın tamamında geçerli: aynı plaka iki şubede görünse bile TEK araç
            // satırı açılır, en eski kayıt kazanır. Plakası boş olan kurye araçsız kalır
            migrationBuilder.Sql(@"
                INSERT INTO Vehicles (Id, Plate, VehicleType, BranchId, IsActive, CreatedDate, IsDeleted)
                SELECT NEWID(), t.Plate, t.VehicleType, t.BranchId, 1, SYSUTCDATETIME(), 0
                FROM (
                    SELECT UPPER(LTRIM(RTRIM(e.VehiclePlate))) AS Plate,
                           e.VehicleType                       AS VehicleType,
                           e.BranchId                          AS BranchId,
                           ROW_NUMBER() OVER (
                               PARTITION BY UPPER(LTRIM(RTRIM(e.VehiclePlate)))
                               ORDER BY e.CreatedDate) AS rn
                    FROM Employees e
                    WHERE e.IsDeleted = 0
                      AND e.VehicleType IS NOT NULL
                      AND e.VehiclePlate IS NOT NULL
                      AND LTRIM(RTRIM(e.VehiclePlate)) <> ''
                ) t
                WHERE t.rn = 1;");

            // Kurye, plakası üzerinden üretilen araca bağlanır
            migrationBuilder.Sql(@"
                UPDATE e
                SET e.VehicleId = v.Id
                FROM Employees e
                INNER JOIN Vehicles v ON v.Plate = UPPER(LTRIM(RTRIM(e.VehiclePlate)))
                WHERE e.IsDeleted = 0
                  AND e.VehiclePlate IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Vehicles_VehicleId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Employees_VehicleId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Employees");
        }
    }
}
