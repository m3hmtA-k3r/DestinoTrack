namespace DestinoTrack.DTO.DTOs.AccountDtos
{
    public class UserCardDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty; 
        public string Role { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } 
        public bool IsManager { get; set; } 
        public bool IsBranchStaff { get; set; } 
        public bool IsCustomer { get; set; } 


    }
}
