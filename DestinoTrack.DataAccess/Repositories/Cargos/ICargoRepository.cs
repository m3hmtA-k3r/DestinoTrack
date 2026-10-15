using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.Cargos
{
    public interface ICargoRepository : IRepository<Cargo>
    {
       
        Task<Cargo?> GetByTrackCodeAsync(string trackCode);
        Task<int> CountByStatusAsync(CargoStatus status);

        // üretilen takip no daha önce kullanılmış mı (silinmişler dahil — numara geri dönüşmez)
        Task<bool> TrackCodeExistsAsync(string trackCode);

        // kapsam sorguda uygulanır — sayfalama ve toplam sayı doğru çıksın
        Task<(List<Cargo> Items, int TotalCount)> GetPagedScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                      Guid? userId, bool unrestricted,
                                                                      CargoStatus? status, string? search, int page, int pageSize);

        // Tek kargo, kapsam içindeyse — kapsam dışındaysa null
        Task<Cargo?> GetScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId, bool unrestricted);


        // detay sayfası — hareketler, teslimat, başarısız denemeler ve şubelerle birlikte  
        Task<Cargo?> GetDetailScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId, bool unrestricted);

        // etiket — şube + şehir + ülke ile birlikte; hareketler gerekmez 
        Task<Cargo?> GetLabelScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId, bool unrestricted);

        // korumayı tahmin edilemez takip numarası sağlar. Çizelge için hareketler de gelir
        Task<Cargo?> GetPublicByTrackCodeAsync(string trackCode);



    }
}
