using DestinoTrack.Entity.Entities;

namespace DestinoTrack.Entity.Entities.Enums
{
    public enum CargoStatus
    {
        Received = 1,                       // Kargo kabul edildi
        InTransferCenter = 2,               // Transfer merkezinde
        DispatchedFromTransferCenter = 3,   // Transfer merkezinden ayrıldı
        ArrivedAtDeliveryBranch = 4,        // Dağıtım şubesine ulaştı
        OutForDelivery = 5,                 // Dağıtıma çıktı
        Delivered = 6                       // Teslim edildi
    }
}


