using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.UserDtos;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    // Kullanıcı listesi + roller + o an seçili filtre
    public class UserIndexViewModel
    {
        public PagedResult<ResultUserDto> Users { get; set; } = new();

        public Dictionary<string, int> RoleCounts { get; set; } = new();

        public string? SelectedRole { get; set; }
        public string? Search { get; set; }

        // Kendi satırında etiket görünür, pasifleştir butonu görünmez 
        public Guid CurrentUserId { get; set; }

        // "Tümü"  sayı: filtreden bağımsız, bütün personel
        public int TotalUserCount => RoleCounts.Values.Sum();

        // Liste boşsa "hiç kayıt yok" ile "filtreye uyan yok" ayrımı için
        public bool IsFiltered => !string.IsNullOrEmpty(SelectedRole) || !string.IsNullOrWhiteSpace(Search);
    }
}
