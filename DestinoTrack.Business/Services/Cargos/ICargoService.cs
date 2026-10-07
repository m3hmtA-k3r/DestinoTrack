using DestinoTrack.DTO.DTOs.CargoDtos;
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


        //fiyat, tahmini teslim ve takip no servis tarafından üretilir; ilk hareket de burada yazılır
        Task<Guid> CreateAsync(CreateCargoDto createCargoDto, Guid performedByUserId);


    }
}
