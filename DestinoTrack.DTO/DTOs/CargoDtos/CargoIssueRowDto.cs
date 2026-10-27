using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Sorunlu kargolar listesinin bir satırı: personel "neden sorunlu" sorusunu
    // satıra bakarak yanıtlayabilmeli
    public class CargoIssueRowDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }
        public CargoStatus CargoStatus { get; set; }

        public string ReceiverName { get; set; }
        public string DestinationBranchName { get; set; }

        // Gecikme buradan görülüyor: tarih geçmişse satır kırmızı çiple işaretlenir
        public DateTime EstimatedArrivalDate { get; set; }

        // Kaç kez denendi ve son denemede ne oldu
        public int AttemptCount { get; set; }
        public DeliveryFailureReason? LastFailureReason { get; set; }

        // Kargo kimin üzerinde (dağıtımdaysa)
        public string? CourierName { get; set; }
    }
}
