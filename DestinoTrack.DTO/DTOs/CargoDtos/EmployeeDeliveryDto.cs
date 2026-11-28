namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class EmployeeDeliveryDto
    {
        public string EmployeeName { get; set; } = string.Empty;

        // Teslim ettiği kargo sayısı: `Deliveries` kargo başına tek satır, bu yüzden satır = kargo
        public int DeliveredCount { get; set; }

        // Başarısız DENEME sayısı, kargo sayısı DEĞİL: aynı kargoya üç kez gidip
        // bulamayan kurye 3 alır. `DeliveryExceptions` kargo başına birden çok satır tutuyor
        public int FailedAttemptCount { get; set; }
    }
}
