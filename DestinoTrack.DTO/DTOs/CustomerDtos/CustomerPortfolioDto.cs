namespace DestinoTrack.DTO.DTOs.CustomerDtos
{
    // Müşteri ekranının üstündeki dört KPI kartı. Dördü de tek sorgudan çıkar
    public class CustomerPortfolioDto
    {
        public int TotalCustomers { get; set; }

        // Kurumsal VE aktif olanlar: "portföy" dediğimiz şey bu
        public int ActiveCorporate { get; set; }

        public int NewThisMonth { get; set; }

        // Onay ekranı olmadığı için bugüne kadar bu sayı hiç azalmadı — ekranın asıl varlık sebebi
        public int PendingApproval { get; set; }
    }
}
