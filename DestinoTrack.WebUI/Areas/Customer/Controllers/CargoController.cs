using DestinoTrack.Business.Services.Cargos;
using DestinoTrack.DataAccess.Interceptors;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Customer.Controllers
{
    public class CargoController(ICargoService _cargoService,
                                 ICurrentUserAccessor _currentUser) : CustomerBaseController
    {
        // Anahtarlar öteki panellerinkinden ayrı: aynı oturumda açık duran
        // iki panel birbirinin filtresini bozmasın diye. Öteki panellerde de benzer bir yaklaşım var.
        private const string DirectionKey = "customer.cargo.direction";
        private const string StatusKey = "customer.cargo.status";
        private const string SearchKey = "customer.cargo.q";

        private bool IsIndividual => _currentUser.CustomerId is null;

        public async Task<IActionResult> Index(int page = 1)
        {
            var direction = IsIndividual ? DirectionFromSession() : null;
            var status = StatusFromSession();
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new CargoListViewModel
            {
                Cargos = await _cargoService.GetPagedAsync(status, search, page, direction),
                SelectedDirection = direction,
                SelectedStatus = status,
                Search = search
            };

            ViewData["ShowDirection"] = IsIndividual;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(CargoDirection? direction, CargoStatus? status, string? q)
        {
            SaveFilter(DirectionKey, IsIndividual ? TextOf(direction) : null);
            SaveFilter(StatusKey, TextOf(status));
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

        private CargoDirection? DirectionFromSession()
        {
            var saved = HttpContext.Session.GetString(DirectionKey);

            if (Enum.TryParse<CargoDirection>(saved, out var direction) && Enum.IsDefined(direction))
            {
                return direction;
            }

            return null;
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

        private static string? TextOf<T>(T? value) where T : struct, Enum
        {
            if (value.HasValue && Enum.IsDefined(value.Value))
            {
                return value.Value.ToString();
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
