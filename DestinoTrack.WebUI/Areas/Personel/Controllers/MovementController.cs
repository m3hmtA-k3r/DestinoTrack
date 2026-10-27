using DestinoTrack.Business;
using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Services.Cargos;
using DestinoTrack.DataAccess.Interceptors;
using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.WebUI.Areas.Personel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Personel.Controllers
{
    // Yeni hareket: ayrı ekran, iki adımlı — takip no → serbest durumlar → açıklama.
    // Serbest durumları ekran kendisi hesaplamıyor, durum makinesinden alıyor
    [Authorize(Roles = RoleNames.Personel)]
    public class MovementController(ICargoService _cargoService,
                                    ICurrentUserAccessor _currentUser,
                                    IStringLocalizer<SharedResource> _localizer) : PersonelBaseController
    {
        public IActionResult Index()
        {
            return View(new MovementViewModel());
        }


        // Numarayı ara: kargo ve gidilebilecek durumlar gelir, sayfa ikinci adıma geçer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find(string? trackCode)
        {
            return await FormView(trackCode, new CreateMovementDto());
        }


        // İkinci adım: hareketi yaz. Geçiş serbest mi — buna servisteki durum makinesi karar veriyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string? trackCode, CreateMovementDto movement)
        {
            if (_currentUser.BranchId == null)
            {
                // Şubesi atanmamış personel hareket yazamaz: hareket bir şubeye kaydedilir
                TempData["Error"] = _localizer["BranchRequired"].Value;

                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Şube ve işlemi yapan kullanıcı formdan GELMEZ — hareket personelin bulunduğu şubeye yazılır
                    await _cargoService.ChangeStatusAsync(movement.CargoId, movement.NewStatus,
                                                          _currentUser.BranchId.Value, _currentUser.UserId!.Value,
                                                          movement.Description);

                    TempData["Success"] = _localizer["MovementSaved"].Value;

                    // Kanıt detayda: yeni hareket çizelgenin en altına düşer
                    return RedirectToAction("Detail", "Cargo", new { id = movement.CargoId });
                }
                catch (ValidationException ex)
                {
                    // Yasak geçiş ya da kapsam dışı kargo: sebebi formun üstünde göster
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return await FormView(trackCode, movement);
        }


        // İkinci adımı her çizişte kargoyu BAŞTAN buluyoruz: formdan gelene güvenmiyoruz,
        // serbest durum listesi de her seferinde durum makinesinden geliyor
        private async Task<IActionResult> FormView(string? trackCode, CreateMovementDto movement)
        {
            var model = new MovementViewModel { TrackCode = trackCode, Movement = movement };

            try
            {
                model.Target = await _cargoService.FindForMovementAsync(trackCode ?? string.Empty);
                model.Movement.CargoId = model.Target.Id;
            }
            catch (ValidationException ex)
            {
                // Bulunamadı ya da kapsam dışı: numarayı silmeden, kutunun altında söyle → birinci adımda kalır
                ModelState.AddModelError(nameof(MovementViewModel.TrackCode), ex.Message);
            }

            return View(nameof(Index), model);
        }
    }
}
