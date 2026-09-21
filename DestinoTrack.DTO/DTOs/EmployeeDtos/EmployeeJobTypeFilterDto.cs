using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.EmployeeDtos
{
    // Personel listesinin üstündeki görev çipleri: "Kurye 12 · Şube Personeli 30 · Depo Görevlisi 8 · Şoför 5"
    public class EmployeeJobTypeFilterDto
    {
        public EmployeeJobType JobType { get; set; }
        public int EmployeeCount { get; set; }
    }
}
