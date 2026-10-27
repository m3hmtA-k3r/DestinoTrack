using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Dağıtım ekranının bir satırı. Genel liste satırından farkı: kuryenin sahada ihtiyacı olan
    // adres/telefon, kargonun kimin üzerinde olduğu ve kapıda tahsil edilecek tutar
    public class DeliveryRowDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }
        public CargoStatus CargoStatus { get; set; }

        // Kurye kapıya bu bilgiyle gidiyor
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }

        public string DestinationBranchName { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }

        // Kapıda ödeme: kurye tahsil etmeden teslim etmesin diye satırda görünür
        public PaymentType PaymentType { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsPaid { get; set; }

        // Dağıtımdaki satırlarda dolu: kargo kimin üzerinde. Teslim/iade bu kurye adına yazılır,
        // kimlik formdan değil buradan gelir
        public Guid? CourierId { get; set; }
        public string? CourierName { get; set; }

        // Kaçıncı deneme — başarısız denemelerin satır sayısından geliyor, ayrı sayaç tutulmuyor
        public int AttemptCount { get; set; }
    }
}
