namespace DestinoTrack.DTO.DTOs.DashboardDtos
{
    public class DashboardSummaryDto
    {
        // Operasyon
        public int TotalCargoCount { get; set; }
        public int OutForDeliveryCount { get; set; }
        public int DeliveredCount { get; set; }
        public int InTransferCenterCount { get; set; }

        // Ağ
        public int CountryCount { get; set; }
        public int CityCount { get; set; }
        public int BranchCount { get; set; }
        public int CourierCount { get; set; }

        // Teslim oranı — kargo yokken sıfıra bölme olmasın diye burada hesaplanıyor
        public double DeliveryRate =>
            TotalCargoCount == 0 ? 0 : Math.Round((double)DeliveredCount / TotalCargoCount * 100, 1);
    }
}
