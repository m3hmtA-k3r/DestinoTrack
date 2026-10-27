namespace DestinoTrack.DTO.DTOs.EmployeeDtos
{
    // Formlardaki personel açılır listesi: sayfasız, ad ve soyad birleşik
    public class EmployeeLookupDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
    }
}
