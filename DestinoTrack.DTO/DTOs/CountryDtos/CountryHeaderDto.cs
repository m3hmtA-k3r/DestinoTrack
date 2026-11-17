namespace DestinoTrack.DTO.DTOs.CountryDtos
{
    // Ülke panelinin başlığı: kullanıcının HANGİ ülkenin sayılarını gördüğünü söyleyen tek satır.
    // Kargo sayıları buraya girmez — onları görünüm bileşenleri kendi servisinden alıyor
    public class CountryHeaderDto
    {
        public string Name { get; set; } = string.Empty;

        // Ülkedeki şube sayısı: şube şehre, şehir ülkeye bağlı (Branch → City → Country)
        public int BranchCount { get; set; }
    }
}
