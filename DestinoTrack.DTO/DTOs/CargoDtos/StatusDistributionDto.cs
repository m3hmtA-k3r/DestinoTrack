namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class StatusDistributionDto
    {
        // `<progress max>` bu sayıyı kullanıyor: çubuk ile yüzde aynı ölçekten okunsun
        public int Total { get; set; }

        // Dokuz durumun DOKUZU da burada, sıfır olanlar dahil: "hiç iade yok" da bir bilgi
        public List<StatusCountDto> Rows { get; set; } = [];
    }
}
