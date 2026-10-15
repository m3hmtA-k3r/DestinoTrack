using DestinoTrack.Business;
using DestinoTrack.Business.Services.Cargos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    //bu ekran yalnız görüntüler — durum değiştirme, dağıtım ve teslim işlemleri personel panelinde
    public class CargoController(ICargoService _cargoService) : AdminBaseController
    {
        // Filtre oturumda tutulur: adres yalnızca sayfa numarası taşır
        private const string StatusKey = "cargo.status";
        private const string SearchKey = "cargo.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            var status = Enum.TryParse<CargoStatus>(HttpContext.Session.GetString(StatusKey), out var s) ? s : (CargoStatus?)null;
            var search = HttpContext.Session.GetString(SearchKey);

            return View(new CargoIndexViewModel
            {
                //servis kapsamı kendisi uyguluyor, controller'ın ek denetim yapmasına gerek yok
                Cargos = await _cargoService.GetPagedAsync(status, search, page),
                SelectedStatus = status,
                Search = search
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(CargoStatus? status, string? q)
        {
            if (status.HasValue && Enum.IsDefined(status.Value))
            {
                HttpContext.Session.SetString(StatusKey, status.Value.ToString());
            }
            else
            {
                HttpContext.Session.Remove(StatusKey);
            }

            var search = q?.Trim();
            if (string.IsNullOrEmpty(search))
            {
                HttpContext.Session.Remove(SearchKey);
            }
            else
            {
                HttpContext.Session.SetString(SearchKey, search);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                return View(await _cargoService.GetDetailAsync(id));
            }
            catch (ValidationException ex)
            {
                // Kapsam dışı ya da olmayan kargo: listeye dön, sebebi bandda göster
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
