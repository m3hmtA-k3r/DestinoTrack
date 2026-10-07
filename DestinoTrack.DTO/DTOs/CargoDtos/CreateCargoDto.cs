using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Kargo kabul formu. Fiyat · desi · tahmini teslim · takip no · barkod servis tarafından üretilir, burada yok
    public class CreateCargoDto
    {
        // Güzergah
        public Guid OriginBranchId { get; set; }
        public Guid DestinationBranchId { get; set; }

        // Gönderi
        public CargoType CargoType { get; set; } = CargoType.Standard;
        public PaymentType PaymentType { get; set; } = PaymentType.SenderPays;

        public double Weight { get; set; }    
        public double Width { get; set; }    
        public double Height { get; set; } 
        public double Length { get; set; }   

        // gönderici: kayıtlıysa hesabı da bağlanır
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }
        public Guid? SenderId { get; set; }

        //  alıcı
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }
        public Guid? ReceiverId { get; set; }

        // kurumsal gönderi — müşteri onaylı ve aktif olmalı
        public Guid? CustomerId { get; set; }
    }
}
