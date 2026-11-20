using DestinoTrack.Business.Services.Cargos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Manager.Controllers
{
    public class CargoController(ICargoService _cargoService) : ManagerBaseController
    {
        // Anahtarlar Admin ve şube panelininkinden ayrı: aynı oturumda açık duran
        // iki panel birbirinin filtresini bozmasın
        private const string StatusKey = "country.cargo.status";
        private const string SearchKey = "country.cargo.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            var status = StatusFromSession();
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new CargoListViewModel
            {
                Cargos = await _cargoService.GetPagedAsync(status, search, page),
                SelectedStatus = status,
                Search = search
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(CargoStatus? status, string? q)
        {
            // Tanımsız enum saklanmaz: adres çubuğundan "99" gelirse filtre bozulmasın
            SaveFilter(StatusKey, status.HasValue && Enum.IsDefined(status.Value) ? status.Value.ToString() : null);
            SaveFilter(SearchKey, q);

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

        private CargoStatus? StatusFromSession()
        {
            var saved = HttpContext.Session.GetString(StatusKey);

            if (Enum.TryParse<CargoStatus>(saved, out var status) && Enum.IsDefined(status))
            {
                return status;
            }

            return null;
        }

        private void SaveFilter(string key, string? value)
        {
            var text = value?.Trim();

            if (string.IsNullOrEmpty(text))
            {
                HttpContext.Session.Remove(key);

                return;
            }

            HttpContext.Session.SetString(key, text);
        }
    }
}
