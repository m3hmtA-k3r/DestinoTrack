namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // VC8'in altı kartı. "Bugün" olanlar hareket kaydından, anlık olanlar kargo durumundan.
    // Geciken ve sorunlu sayıları TALP-79'un kuralından geçer, burada yeniden hesaplanmaz
    public class BranchDailySummaryDto
    {
        public int ArrivedToday { get; set; }     // bugün bu şubeye ulaşan
        public int DispatchedToday { get; set; }  // bugün bu gişeden kabul edilip yola çıkan
        public int DeliveredToday { get; set; }   // bugün bu şubede teslim edilen

        public int OnDelivery { get; set; }       // şu an kuryelerin üzerinde
        public int Delayed { get; set; }          // D66 kuralı
        public int Issues { get; set; }           // üç sorun kümesinin birleşimi
    }
}
