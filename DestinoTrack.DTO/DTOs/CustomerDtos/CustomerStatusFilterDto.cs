using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CustomerDtos
{
    // Listenin üstündeki durum çipleri: "Aktif 120 · Onay bekliyor 14 · Askıda 3 · Kapalı 0".
    // Sıfır olanlar da gösterilir (personel çiplerindeki kural) — boş küme de bilgidir
    public class CustomerStatusFilterDto
    {
        public AccountStatus Status { get; set; }
        public int CustomerCount { get; set; }
    }
}
