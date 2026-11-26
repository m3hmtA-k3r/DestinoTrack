namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class ReasonCountDto
    {
        // Enum'un ADI; etiket, karttaki ön ekle birleştirilerek basılıyor
        public string Reason { get; set; } = string.Empty;

        public int Count { get; set; }
        public double Share { get; set; }
    }
}
