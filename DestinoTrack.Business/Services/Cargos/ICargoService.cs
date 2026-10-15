using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Services.Cargos
{
    public interface ICargoService
    {
        // bu geçiş serbest mi yoksa ekranlar düğmeyi buna göre gösterir
        bool CanTransition(CargoStatus from, CargoStatus to);


        //tek kapı — geçişi doğrular, CargoMovement yazar, kargoyu günceller
        Task ChangeStatusAsync(Guid cargoId, CargoStatus newStatus, Guid branchId, Guid performedByUserId,
                               string? description = null, DelayReason? delayReason = null);


        // kargoyu kuryeye atar, kodu üretir (ilk çıkışta) ve dağıtıma çıkarır — üretilen kodu döner
        Task<string> DispatchAsync(Guid cargoId, Guid courierEmployeeId, Guid branchId, Guid performedByUserId);


        // kod doğrulanmadan teslim yok · teslim alan kişi ve kurye Delivery'ye yazılır
        Task DeliverAsync(Guid cargoId, string deliveryCode, string recipientName, Guid courierEmployeeId,
                          Guid branchId, Guid performedByUserId);


        // başarısız denemeyi yazar; sınıra ulaşılınca iade sürecini kendiliğinden başlatır
        Task FailDeliveryAsync(Guid cargoId, DeliveryFailureReason reason, string? description, Guid courierEmployeeId,
                               Guid branchId, Guid performedByUserId);



        //fiyat, tahmini teslim ve takip no servis tarafından üretilir; ilk hareket de burada yazılır
        Task<Guid> CreateAsync(CreateCargoDto createCargoDto, Guid performedByUserId);

        // yalnızca kullanıcının kapsamındaki kargolar
        Task<PagedResult<ResultCargoDto>> GetPagedAsync(CargoStatus? status = null, string? search = null, int page = 1);


        // detay + hareket çizelgesi (kapsam dışındaysa yetki hatası)
        Task<CargoDetailDto> GetDetailAsync(Guid id);

        // yazdırılabilir etiketin verisi + hazır barkod/QR (kapsam dışındaysa yetki hatası)
        Task<CargoLabelDto> GetLabelAsync(Guid id);

        // Girişsiz takip kapsam uygulanmaz, koruma tahmin edilemez takip numarasında.
        // Biçim tutmuyorsa ya da kayıt yoksa null döner — sayfa "bulunamadı" der, hata fırlatmaz
        Task<PublicTrackingDto?> GetPublicTrackingAsync(string trackCode);


    }
}
