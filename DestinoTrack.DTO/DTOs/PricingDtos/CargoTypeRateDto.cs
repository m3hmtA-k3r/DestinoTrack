using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.PricingDtos
{
    // Sekiz tip satır içinde düzenlenir: ekleme ve silme yok, yalnız değer değişir
    public class CargoTypeRateDto
    {
        public Guid Id { get; set; }
        public CargoType CargoType { get; set; }

        public decimal Multiplier { get; set; }
        public int TransitDaysDelta { get; set; }
    }
}
