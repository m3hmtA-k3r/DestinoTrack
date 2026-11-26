using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class StatusCountDto
    {
        public CargoStatus Status { get; set; }
        public int Count { get; set; }

        // 0..1 arası pay. Yüzde metni buradan basılıyor; çubuk ise Count/Total'dan
        public double Share { get; set; }
    }
}
