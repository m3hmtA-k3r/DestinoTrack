using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Manager.Controllers
{
    public class CargoController(ICargoService _cargoService) : ManagerBaseController
    {
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                return View(await _cargoService.GetDetailAsync(id));
            }
            catch (ValidationException ex)
            {
                // Kapsam dışı ya da olmayan kargo: geldiği ekrana dön, sebebi bandda göster
                TempData["Error"] = ex.Message;

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
