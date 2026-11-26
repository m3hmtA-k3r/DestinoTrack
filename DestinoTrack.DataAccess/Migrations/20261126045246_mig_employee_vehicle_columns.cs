using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DestinoTrack.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig_employee_vehicle_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehiclePlate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VehiclePlate",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                table: "Employees",
                type: "int",
                nullable: true);

            // Geri alma yalnız şemayı değil veriyi de döndürür: araç havuzu bu noktada hâlâ duruyor
            // (onu 13. migration kaldırır), bu yüzden plaka ve tür bağlı araçtan geri yazılabilir
            migrationBuilder.Sql(@"
                UPDATE e
                SET e.VehiclePlate = v.Plate,
                    e.VehicleType  = v.VehicleType
                FROM Employees e
                INNER JOIN Vehicles v ON v.Id = e.VehicleId;");
        }
    }
}
