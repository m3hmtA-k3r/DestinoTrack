using DestinoTrack.Business;
using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Business.Services.Cities;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    // Transfer merkezi ekranı: yalnızca BranchType.TransferCenter · tür çipi yok · yeni kayıt otomatik transfer merkezi
    public class TransferCenterController(IBranchService branchService, ICityService cityService,
                                          ICountryService countryService, IStringLocalizer<SharedResource> localizer)
        : BranchControllerBase(branchService, cityService, countryService, localizer)
    {
        protected override BranchType? LockedType => BranchType.TransferCenter;
        protected override string SessionPrefix => "tc";
    }
}
