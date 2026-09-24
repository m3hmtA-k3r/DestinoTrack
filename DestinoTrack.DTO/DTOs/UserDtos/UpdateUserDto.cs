namespace DestinoTrack.DTO.DTOs.UserDtos
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public Guid? CountryId { get; set; }   // Manager için zorunlu
        public Guid? BranchId { get; set; }    // Personel / Courier — atanmamış olabilir


        // hesabın Employee kaydı varsa şube buradan değiştirilemez — form kutuyu kilitler, servis gelen değeri yok sayar
        public bool HasEmployeeRecord { get; set; }


        // ikisi de boşsa şifre değişmez; doluysa kuralıyla yeni şifre atanır
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
