namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class TopBranchesDto
    {
        // Çubuğun ölçeği: en yoğun şube. Karşılaştırma şubeler arası
        public int Max { get; set; }

        // En yoğun beş şube; sistemde beşten az şube varsa daha kısa olabilir
        public List<BranchTrafficDto> Rows { get; set; } = [];
    }
}
