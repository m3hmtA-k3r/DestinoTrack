using DestinoTrack.Business;
using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Services.Cargos;
using DestinoTrack.Business.Services.Employees;
using DestinoTrack.DataAccess.Interceptors;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Personel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Personel.Controllers
{
    // Dağıtım listesi: tek ekran, iki bölüm (D72). Taban controller Personel VE Kurye'yi alır (D63) —
    // kurye kendi kargolarını kapsam sayesinde görür (D73), ayrı bir süzgece gerek yok
    public class DeliveryController(ICargoService _cargoService,
                                    IEmployeeService _employeeService,
                                    ICurrentUserAccessor _currentUser,
                                    IStringLocalizer<SharedResource> _localizer) : PersonelBaseController
    {
        public async Task<IActionResult> Index(int page = 1, int hazirSayfa = 1)
        {
            var personelMi = User.IsInRole(RoleNames.Personel);

            var model = new DeliveryIndexViewModel
            {
                OnDelivery = await _cargoService.GetOnDeliveryAsync(page),

                // Üst bölüm yalnız personelde: kurye kendine kargo atamaz
                ReadyForDispatch = personelMi ? await _cargoService.GetReadyForDispatchAsync(hazirSayfa) : null,
                Couriers = personelMi && _currentUser.BranchId.HasValue
                    ? await _employeeService.GetCourierLookupAsync(_currentUser.BranchId.Value)
                    : []
            };

            return View(model);
        }


        // Dağıtıma çıkar — yalnız Personel. Taban Personel+Kurye'yi alır, bu eylem daraltır (R4)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Personel)]
        public async Task<IActionResult> Dispatch(Guid cargoId, Guid courierId)
        {
            return await Calistir(() => _cargoService.DispatchAsync(cargoId, courierId,
                                                                    _currentUser.BranchId!.Value, _currentUser.UserId!.Value),
                                  "CargoDispatched");
        }


        // Teslim et — kodu alıcı söyler, kurye yazar. Kurye kimliği satırdan gelir;
        // yanlış gönderilirse DeliverAsync "bu kargo bu kuryeye atanmamış" diye reddeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deliver(Guid cargoId, Guid courierId, string? code, string? recipientName)
        {
            return await Calistir(() => _cargoService.DeliverAsync(cargoId, code ?? string.Empty, recipientName ?? string.Empty,
                                                                   courierId, _currentUser.BranchId!.Value, _currentUser.UserId!.Value),
                                  "MovementSaved");
        }


        // Teslim edilemedi — gerekçe zorunlu, "Diğer" ise açıklama da zorunlu (servis denetliyor)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fail(Guid cargoId, Guid courierId, DeliveryFailureReason reason, string? description)
        {
            return await Calistir(() => _cargoService.FailDeliveryAsync(cargoId, reason, description, courierId,
                                                                        _currentUser.BranchId!.Value, _currentUser.UserId!.Value),
                                  "MovementSaved");
        }


        // Üç satır içi işlemin ortak kabuğu: şube denetimi, servisin reddi ve sonuç bandı tek yerde.
        // Satır içi işlemler form yeniden çizmiyor — listeye dönüp sonucu bantta söylüyoruz
        private async Task<IActionResult> Calistir(Func<Task> islem, string basariAnahtari)
        {
            if (_currentUser.BranchId == null)
            {
                // Hareket bir şubeye yazılır; şubesi atanmamış kullanıcı bu işlemleri yapamaz
                TempData["Error"] = _localizer["BranchRequired"].Value;

                return RedirectToAction(nameof(Index));
            }

            try
            {
                await islem();

                TempData["Success"] = _localizer[basariAnahtari].Value;
            }
            catch (ValidationException ex)
            {
                // Yanlış teslim kodu · başka kuryenin kargosu · pasif kurye · kapsam dışı kayıt
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
