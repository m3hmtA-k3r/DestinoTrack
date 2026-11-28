using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.CustomerDtos;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Services.Cargos
{
    public interface ICargoService
    {
        // bu geçiş serbest mi yoksa ekranlar düğmeyi buna göre gösterir
        bool CanTransition(CargoStatus from, CargoStatus to);


        // Yeni hareket ekranının birinci adımı: numarayla kargoyu bul, o an gidilebilecek durumları da getir
        Task<CargoMovementTargetDto> FindForMovementAsync(string trackCode);


        Task ChangeStatusAsync(Guid cargoId, CargoStatus newStatus, Guid branchId, Guid performedByUserId,
                               string? description = null, DelayReason? delayReason = null);


        Task<string> DispatchAsync(Guid cargoId, Guid courierEmployeeId, Guid branchId, Guid performedByUserId);


        Task DeliverAsync(Guid cargoId, string deliveryCode, string recipientName, Guid courierEmployeeId,
                          Guid branchId, Guid performedByUserId);


        Task FailDeliveryAsync(Guid cargoId, DeliveryFailureReason reason, string? description, Guid courierEmployeeId,
                               Guid branchId, Guid performedByUserId);



        //fiyat, tahmini teslim ve takip no servis tarafından üretilir; ilk hareket de burada yazılır
        Task<Guid> CreateAsync(CreateCargoDto createCargoDto, Guid performedByUserId);

        // Gişe formundaki müşteri açılır listesi — yalnız Active (D109), yalnız verilen ülke (D110), ünvana göre sıralı.
        // countryId çağırandan gelir: personelin AppUser.CountryId'si null, ülke kendi şubesinden çıkarılır
        Task<List<CustomerLookupDto>> GetCustomerLookupAsync(Guid countryId);

        // yalnızca kullanıcının kapsamındaki kargolar · direction yalnız şube kapsamında anlamlı (gelen / çıkan)
        Task<PagedResult<ResultCargoDto>> GetPagedAsync(CargoStatus? status = null, string? search = null, int page = 1,
                                                        CargoDirection? direction = null);

        Task<PagedResult<DeliveryRowDto>> GetReadyForDispatchAsync(int page = 1);
        Task<PagedResult<DeliveryRowDto>> GetOnDeliveryAsync(int page = 1);

        Task<PagedResult<CargoIssueRowDto>> GetIssuesAsync(CargoIssue? issue = null, int page = 1);


        Task<DailySummaryDto?> GetDailySummaryAsync();

        Task<List<RecentMovementDto>> GetRecentMovementsAsync(int count = 8);

        Task<CustomerSummaryDto?> GetCustomerSummaryAsync();
        Task<CustomerTimelineDto?> GetCustomerTimelineAsync();

        // Admin panosu: kısıtsız kapsam dışına null
        Task<AdminSummaryDto?> GetAdminSummaryAsync();
        Task<StatusDistributionDto?> GetStatusDistributionAsync();

        // Admin panosu · son 7 gün trafiği: yedi gün, kargosuz günler dahil
        Task<WeeklyTrafficDto?> GetWeeklyTrafficAsync();


        // Admin panosu · en yoğun şubeler (varsayılan beş)
        Task<TopBranchesDto?> GetTopBranchesAsync(int take = 5);

        // Rapor sayfası · SLA: zamanında teslim oranı (payda YALNIZ teslim edilenler)
        Task<SlaSummaryDto?> GetSlaSummaryAsync();

        // Rapor · şubeler geciken kargo sayısına göre azalan (varsayılan beş).
        // Boş liste "hiç geciken yok" demek — bu iyi haber, görünüm öyle de basacak
        Task<DelayedBranchesDto?> GetDelayedBranchesAsync(int take = 5);


        // Rapor · personel başına teslim ve başarısız deneme (varsayılan beş).
        // "Başarısız" DENEME sayar, kargo sayar gibi okunmamalı (D108)
        Task<DeliveryPerformanceDto?> GetDeliveryPerformanceAsync(int take = 5);

        // Rapor · kök neden dağılımları: beş nedenin beşi de satır alır, sıfırlar dahil
        Task<ReasonBreakdownDto?> GetDelayReasonBreakdownAsync();
        Task<ReasonBreakdownDto?> GetFailureReasonBreakdownAsync();
        Task<CargoDetailDto> GetDetailAsync(Guid id);

        // yazdırılabilir etiketin verisi + hazır barkod/QR (kapsam dışındaysa yetki hatası)
        Task<CargoLabelDto> GetLabelAsync(Guid id);

        // Girişsiz takip kapsam uygulanmaz, koruma tahmin edilemez takip numarasında.
        // Biçim tutmuyorsa ya da kayıt yoksa null döner — sayfa "bulunamadı" der, hata fırlatmaz
        Task<PublicTrackingDto?> GetPublicTrackingAsync(string trackCode);


    }
}
