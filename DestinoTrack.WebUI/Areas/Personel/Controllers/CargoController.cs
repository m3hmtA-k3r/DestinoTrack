using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Business.Services.Cargos;
using DestinoTrack.Business.Services.Pricing;
using DestinoTrack.DataAccess.Interceptors;
using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.PricingDtos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Personel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


using DestinoTrack.Business;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.WebUI.Areas.Personel.Controllers
{
    [Authorize(Roles = RoleNames.Personel)]
    public class CargoController(ICargoService _cargoService,
                                 IBranchService _branchService,
                                 ICargoPricingService _pricingService,
                                 ICurrentUserAccessor _currentUser,
                                 IStringLocalizer<SharedResource> _localizer) : PersonelBaseController


    {
        // Anahtarlar Admin ekranınınkinden ayrı: iki panel aynı oturumda birbirinin filtresini bozmasın diye yaptık
        private const string DirectionKey = "staff.cargo.direction";
        private const string StatusKey = "staff.cargo.status";
        private const string SearchKey = "staff.cargo.q";


        public async Task<IActionResult> Index(int page = 1)
        {
            var direction = DirectionFromSession();
            var status = StatusFromSession();
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new CargoIndexViewModel
            {
                // Kapsamı servis uyguluyor: personel yalnız kendi şubesine değen kargoları görür 
                Cargos = await _cargoService.GetPagedAsync(status, search, page, direction),
                SelectedDirection = direction,
                SelectedStatus = status,
                Search = search
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(CargoDirection? direction, CargoStatus? status, string? q)
        {
            SaveFilter(DirectionKey, TextOf(direction));
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


        // kargo kabul formu. Çıkış şubesi seçilmez, personelin kendi şubesidir
        public async Task<IActionResult> Create()
        {
            return await FormView(new CreateCargoDto());
        }


        //  kaydetmeden fiyat sorar — kayıt açılmaz, aynı sayfa dolu form ve tutarla yeniden çizilir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Quote(CreateCargoDto cargo)
        {
            // Fiyat sorusu bir kayıt denemesi değil: henüz doldurulmamış alanlar için uyarı çıkmasın
            ModelState.Clear();

            return await FormView(cargo, fiyatSor: true);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCargoDto cargo)
        {
            if (_currentUser.BranchId == null)
            {
                // Şubesi atanmamış personel kargo kabul edemez — formu açarken de aynı kural geçerli
                TempData["Error"] = _localizer["BranchRequired"].Value;

                return RedirectToAction(nameof(Index));
            }

            //  çıkış şubesi formdan GELMEZ, sunucuda yazılır — kullanıcı başka şube adına kargo açamasın
            cargo.OriginBranchId = _currentUser.BranchId.Value;

            // Doğrulama eylemden önce koştuğu için çıkış şubesi o anda boştu ve model geçersiz sayılıyordu.
            // Alan sunucuda dolduruldu; kaydedilecek hâlini yeniden doğruluyoruz
            ModelState.Clear();
            TryValidateModel(cargo, nameof(cargo));

            if (!ModelState.IsValid)
            {
                return await FormView(cargo);
            }

            try
            {
                var id = await _cargoService.CreateAsync(cargo, _currentUser.UserId!.Value);

                return RedirectToAction(nameof(Detail), new { id });
            }
            catch (ValidationException ex)
            {
                // Servisin kuralı (tarife yok · müşteri onaysız · ölçü geçersiz) forma döner
                ModelState.AddModelError(string.Empty, ex.Message);

                return await FormView(cargo);
            }
        }



        // Formun her hâlinde aynı hazırlık: şube listesi + çıkış şubesinin adı (+ istenirse fiyat)
        private async Task<IActionResult> FormView(CreateCargoDto cargo, bool fiyatSor = false)
        {
            var subeId = _currentUser.BranchId;

            if (subeId == null)
            {
                // Şubesi atanmamış personel kargo kabul edemez — sessizce boş form göstermek yanıltıcı olur
                TempData["Error"] = _localizer["BranchRequired"].Value;

                return RedirectToAction(nameof(Index));
            }

            var subeler = await _branchService.GetLookupAsync();

            var model = new CargoCreateViewModel
            {
                Cargo = cargo,
                OriginBranchName = subeler.FirstOrDefault(b => b.Id == subeId.Value)?.Name,

                // Kendi şubesi varış olarak seçilemez: aynı şubeye gönderi operasyonda anlamsız
                DestinationBranches = subeler.Where(b => b.Id != subeId.Value).ToList()
            };

            if (fiyatSor && cargo.DestinationBranchId != Guid.Empty)
            {
                model.Quote = await _pricingService.GetQuoteAsync(new CargoQuoteRequestDto
                {
                    OriginBranchId = subeId.Value,
                    DestinationBranchId = cargo.DestinationBranchId,
                    CargoType = cargo.CargoType,
                    Weight = cargo.Weight,
                    Width = cargo.Width,
                    Height = cargo.Height,
                    Length = cargo.Length
                });
            }

            return View(nameof(Create), model);
        }



        // ---- Oturumdaki filtre ----

        // Oturumdaki metin geçerli bir yön değilse (eski kayıt, elle yazılmış adres) filtre yokmuş gibi davranır ve null doner
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

        // Tanımsız enum saklanmaz: adres çubuğundan "99" gelirse filtre bozulmasın.
        private static string? TextOf<T>(T? value) where T : struct, Enum
        {
            if (value.HasValue && Enum.IsDefined(value.Value))
            {
                return value.Value.ToString();
            }

            return null;
        }

        // Değer varsa oturumda saklanır, yoksa filtre kaldırılır — üç filtre de aynı kuralla çalışıyr suan için 
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
