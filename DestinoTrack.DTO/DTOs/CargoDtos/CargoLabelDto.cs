using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Yazdırılabilir kargo etiketinin taşıdığı her şey 
    // Görseller hazır SVG olarak gelir; sayfa hesap yapmaz, yalnız yerleştirir
    public class CargoLabelDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }
        public string Barcode { get; set; }        // barkodun altına basılan yazı
        public string BarcodeSvg { get; set; }
        public string QrSvg { get; set; }
        public string QrContent { get; set; }      // QR'ın taşıdığı metin: takip no ya da takip adresi
        public CargoType CargoType { get; set; }

        // Taraflar
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }
        public string? CustomerTitle { get; set; }       
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }

        // Güzergah — ayrıştırma bandında okunan asıl bilgi
        public string OriginBranchName { get; set; }
        public string OriginBranchCode { get; set; }
        public string OriginCityName { get; set; }
        public string DestinationBranchName { get; set; }
        public string DestinationBranchCode { get; set; }
        public string DestinationCityName { get; set; }
        public string DestinationCountryName { get; set; }

        // Ölçüler — ücretin dayanağı, itirazda bakılan satır
        public double Weight { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double Desi { get; set; }

        // Ücret — alıcı ödemeli gönderide kurye bu satıra bakar
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool IsPaid { get; set; }

        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }

        // Etiketin basıldığı an — aynı kargonun iki etiketi karışırsa hangisi yeni, belli olur
        public DateTime PrintedAt { get; set; }
    }
}
