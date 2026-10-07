using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    // Kargo tipinin fiyata ve süreye etkisi 
    public class CargoTypeRate : BaseEntity
    {
        public CargoType CargoType { get; set; }

        // Rota tarifesinden çıkan ücret bununla çarpılır: 1.00 = değişiklik yok, 1.50 = %50 fazla
        public decimal Multiplier { get; set; }

        // Teslim gününe eklenir; eksi olabilir (Acil = -1). Sonuç en az 1 güne sabitlenir
        public int TransitDaysDelta { get; set; }
    }
}
