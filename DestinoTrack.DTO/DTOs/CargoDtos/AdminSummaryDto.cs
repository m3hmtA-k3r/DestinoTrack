namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class AdminSummaryDto
    {
        public int TotalCargo { get; set; }

        // Durum makinesinden türetiliyor: hedefi olan durumlar
        public int ActiveCargo { get; set; }

        // "Geciken" tanımı TEK yerde: CargoRepository.WithIssue
        public int DelayedCargo { get; set; }

        // Oranın payı ve paydası: payda son duraklar (Delivered + ReturnedToSender)
        public int DeliveredCargo { get; set; }
        public int FinalizedCargo { get; set; }

        // Hiç sonuçlanmamışsa oran YOK — 0 yazmak "hepsi başarısız" demek olurdu 
        public double? DeliveryRate { get; set; }
    }
}
