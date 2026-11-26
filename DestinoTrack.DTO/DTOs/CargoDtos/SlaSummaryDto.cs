namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class SlaSummaryDto
    {
        // Payda: YALNIZ teslim edilenler. İade edilen hiç teslim edilmedi, ona
        // "zamanında mıydı" sorusu sorulmaz — panodaki teslimat oranından farklı sayı
        public int Delivered { get; set; }

        public int OnTime { get; set; }
        public int Late { get; set; }

        // Hiç teslim edilmiş kargo yoksa oran YOK — ekranda tire
        public double? OnTimeRate { get; set; }
    }
}
