namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class BranchTrafficDto
    {
        public string BranchName { get; set; } = string.Empty;

        // Şubeye DEĞEN kargo: çıkış veya varış. Scoped'in şube dalıyla aynı tanım ,
        // böylece bu sayı şubenin kendi panosundaki toplamla eşit çıkıyor
        public int Count { get; set; }
    }
}
