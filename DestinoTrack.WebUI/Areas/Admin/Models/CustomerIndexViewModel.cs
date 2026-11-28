using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.CustomerDtos;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    // Müşteri listesi ekranı: sayfa + durum çipleri + seçili süzgeçler.
    // KPI kartları burada YOK — ayrı ViewComponent, verisini kendisi alıyor (AdminSummary kalıbı)
    public class CustomerIndexViewModel
    {
        public PagedResult<ResultCustomerDto> Customers { get; set; } = new();

        // Dört durumun hepsi, sayısı sıfır olanlar dahil
        public List<CustomerStatusFilterDto> StatusFilters { get; set; } = new();

        // Seçili süzgeçler: çipin ve açılır listenin hangi seçenekte durduğunu ekran buradan bilir
        public AccountStatus? SelectedStatus { get; set; }
        public CustomerType? SelectedType { get; set; }
        public string? Search { get; set; }

        // "Tümü" çipinin sayısı: süzgeçten bağımsız, bütün müşteriler
        public int TotalCustomerCount => StatusFilters.Sum(f => f.CustomerCount);

        // Boş liste iki ayrı sebepten doğar: hiç kayıt yok ya da süzgeç eşleşmedi.
        // İkisinde farklı ekran gösterilecek, karar bu alanda
        public bool IsFiltered => SelectedStatus.HasValue || SelectedType.HasValue || !string.IsNullOrWhiteSpace(Search);
    }
}
