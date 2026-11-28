using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CustomerDtos
{
    // Müşteri listesinin bir satırı. Gönderi sayısı ve son gönderi tarihi listede var ama formda yok:
    // ikisi de hesaplanan değer, kullanıcı giremez
    public class ResultCustomerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string TaxNumber { get; set; }

        public CustomerType CustomerType { get; set; }
        public AccountStatus Status { get; set; }

        public string CountryName { get; set; }

        public int CargoCount { get; set; }

        // Hiç gönderi yoksa null — ekranda "—" yazılacak, "01.01.0001" değil
        public DateTime? LastCargoDate { get; set; }
    }
}
