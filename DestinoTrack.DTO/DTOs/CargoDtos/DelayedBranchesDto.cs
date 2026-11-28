namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class DelayedBranchesDto
    {
        // Çubuğun ölçeği: en çok geciken şube. Liste boşsa 0 kalır, görünüm bunu boş duruma çevirir
        public int Max { get; set; }

        // En çok gecikeni olan şubeler, azalan sıralı
        public List<BranchDelayDto> Rows { get; set; } = [];
    }
}
