namespace DestinoTrack.DTO.DTOs.Common
{
    // Sayfa bilgisi — kayıt türünden bağımsız. Sayfalama çubuğu yalnızca bunu tanır,
    // böylece şehir · ülke · kullanıcı listelerinde aynı partial kullanılır
    public abstract class PagedResultBase
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        // 151 kayıt / 20 = 7,55 → 8 sayfa (yukarı yuvarlanır)
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        // "141 kayıttan 41–60 arası" satırı için
        public int FirstRow => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int LastRow => Math.Min(Page * PageSize, TotalCount);
    }

    // Sayfa bilgisi + o sayfanın kayıtları
    public class PagedResult<T> : PagedResultBase
    {
        public List<T> Items { get; set; } = new();
    }
}
