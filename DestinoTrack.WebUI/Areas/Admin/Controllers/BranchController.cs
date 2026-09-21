using DestinoTrack.Business;
using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Business.Services.Cities;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    // Şube ekranı: bütün tesis türleri, tür çipleriyle süzülür (işin tamamı BranchControllerBase'te)
    public class BranchController(IBranchService branchService, ICityService cityService,
                                  ICountryService countryService, IStringLocalizer<SharedResource> localizer)
        : BranchControllerBase(branchService, cityService, countryService, localizer)
    {
        protected override BranchType? LockedType => null;
        protected override string SessionPrefix => "branch";
    }
}
