namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class DeliveryPerformanceDto
    {
        // Çubuğun ölçeği: en çok teslim eden personel. Liste boşsa 0,
        // liste dolu ama herkesin teslimi 0 ise en az 1 (max="0" geçersiz HTML olurdu)
        public int Max { get; set; }

        // Personel, teslim sayısına göre azalan sıralı
        public List<EmployeeDeliveryDto> Rows { get; set; } = [];
    }
}
