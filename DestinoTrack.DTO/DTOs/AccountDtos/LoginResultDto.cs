namespace DestinoTrack.DTO.DTOs.AccountDtos
{
    public class LoginResultDto
    {

        // Genel hata 15 dakikalık kilit pasif hesabın hangisi olduğuna servis karar verir
        public string? ErrorMessage { get; set; }

        // Admin → Admin paneli, diğer roller → Hesabım 
        public bool IsAdmin { get; set; }

        // Manager → ülke paneli (R8: her rol kendi dashboard'uyla karşılanır)
        public bool IsManager { get; set; }

        // Personel · Kurye → şube paneli (RoleNames.BranchPanel ile aynı küme).
        // Kararı servis veriyor; controller yalnızca nereye gideceğine bakar
        public bool IsBranchStaff { get; set; }
        public bool IsCustomer { get; set; }
        public bool Succeeded { get; set; }

    }
}
