namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class ReasonBreakdownDto
    {
        // Kartın başlık anahtarı ve etiket ön eki: iki kart aynı görünümü kullanıyor,
        // ikinci bir kopya yazılmıyor
        public string TitleKey { get; set; } = string.Empty;
        public string LabelPrefix { get; set; } = string.Empty;

        public int Total { get; set; }

        // Çubuğun ölçeği: en büyük neden. Karşılaştırma nedenler arası
        public int Max { get; set; }

        // Beş nedenin beşi de burada, sıfırlar dahil
        public List<ReasonCountDto> Rows { get; set; } = [];
    }
}
