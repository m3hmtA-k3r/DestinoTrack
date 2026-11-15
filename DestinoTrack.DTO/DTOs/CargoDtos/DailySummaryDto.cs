namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class DailySummaryDto
    {
        public int ArrivedToday { get; set; }     // varış hareketi yazılan
        public int DispatchedToday { get; set; }  // gönderici şubesinde hareketi yazılan
        public int DeliveredToday { get; set; }   // teslim edilen

        // Anlık olanlar kargonun o anki durumundan
        public int OnDelivery { get; set; }       // şu an kuryelerin üzerinde
        public int Delayed { get; set; }          
        public int Issues { get; set; }           // üç sorun kümesinin birleşimi
    }
}
