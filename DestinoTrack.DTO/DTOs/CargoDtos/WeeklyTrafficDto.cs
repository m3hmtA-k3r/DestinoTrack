namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class WeeklyTrafficDto
    {
        public int Max { get; set; }

        // Yedi gün, kargosuz günler dahil: atlanan gün grafiği yalancı yapar
        public List<DailyTrafficDto> Rows { get; set; } = [];
    }
}
