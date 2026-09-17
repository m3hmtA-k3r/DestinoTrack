using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DestinoTrack.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig_cargo_movement_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CargoMovements_Cargos_CargoId",
                table: "CargoMovements");

            migrationBuilder.AddColumn<int>(
                name: "OldStatus",
                table: "CargoMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PerformedByUserId",
                table: "CargoMovements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CargoMovements_PerformedByUserId",
                table: "CargoMovements",
                column: "PerformedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CargoMovements_AspNetUsers_PerformedByUserId",
                table: "CargoMovements",
                column: "PerformedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CargoMovements_Cargos_CargoId",
                table: "CargoMovements",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CargoMovements_AspNetUsers_PerformedByUserId",
                table: "CargoMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_CargoMovements_Cargos_CargoId",
                table: "CargoMovements");

            migrationBuilder.DropIndex(
                name: "IX_CargoMovements_PerformedByUserId",
                table: "CargoMovements");

            migrationBuilder.DropColumn(
                name: "OldStatus",
                table: "CargoMovements");

            migrationBuilder.DropColumn(
                name: "PerformedByUserId",
                table: "CargoMovements");

            migrationBuilder.AddForeignKey(
                name: "FK_CargoMovements_Cargos_CargoId",
                table: "CargoMovements",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
