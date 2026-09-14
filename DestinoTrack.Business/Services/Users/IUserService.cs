using DestinoTrack.DTO.DTOs.UserDtos;
using Microsoft.AspNetCore.Identity;

namespace DestinoTrack.Business.Services.Users
{
    public interface IUserService
    {
        // Yalnızca personel (RoleNames.Staff); rol ve arama isteğe bağlı — ikisi de boşsa hepsi
        Task<List<ResultUserDto>> GetAllAsync(string? role = null, string? search = null);

        // Rol çipleri için: her personel rolünde kaç kullanıcı var (0 olanlar dahil)
        Task<Dictionary<string, int>> GetRoleCountsAsync();

        Task<UpdateUserDto> GetByIdAsync(Guid id);

        // Identity kuralına takılırsa hatalar IdentityResult içinde döner — controller forma yazar
        Task<IdentityResult> CreateAsync(CreateUserDto createUserDto);
        Task<IdentityResult> UpdateAsync(UpdateUserDto updateUserDto, Guid currentUserId);

        //pasifleştir / aktifleştir 
        Task<IdentityResult> SetActiveAsync(Guid id, bool isActive, Guid currentUserId);

        // Giriş kilitliyse gösterilecek mesaj: pasif hesap mı, süreli kilit mi
        Task<string> GetLockedOutMessageAsync(string email, string password);
    }
}
