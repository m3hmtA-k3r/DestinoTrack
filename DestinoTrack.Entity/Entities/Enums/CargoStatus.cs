namespace DestinoTrack.Entity.Entities.Enums
{
    //Kargonun yaşam döngüsü Değerler veritabanına sayı olarak yazılır: numaralar bir kez verilir, sonradan değiştirilmez
    public enum CargoStatus
    {
        // Ana akış
        Created = 1,  // Oluşturuldu
        AtOriginBranch = 2, // Gönderici şubesinde
        InTransferCenter = 3, // Transfer merkezinde
        AtDestinationBranch = 4,// Varış şubesinde
        OutForDelivery = 5,  // Dağıtıma çıktı
        Delivered = 6,   // Teslim edildi

        // Alternatif akış
        DeliveryFailed = 7,  // Teslim edilemedi → yeniden dağıtım
        ReturnInProgress = 8, // İade sürecinde (3 başarısız deneme sonrası)
        ReturnedToSender = 9  // Göndericiye iade edildi
    }
}
