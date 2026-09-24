using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    // VC — tesis özeti kartı: veriyi controller'dan değil servisten alır 
    public class BranchSummaryViewComponent(IBranchService _branchService) : ViewComponent
    {
        // branchType: ekranın kapsamı — null = tüm tesisler · TransferCenter = yalnız transfer merkezleri
        // dashboard aynı veriyi başka bir görünümle çizecek (AdminUserCard'daki kalıp)
        public async Task<IViewComponentResult> InvokeAsync(BranchType? branchType = null, string view = "Default")
        {
            var summary = await _branchService.GetSummaryAsync(branchType);

            return View(view, summary);
        }
    }
}
