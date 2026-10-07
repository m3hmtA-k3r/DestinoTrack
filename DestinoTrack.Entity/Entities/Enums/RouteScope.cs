namespace DestinoTrack.Entity.Entities.Enums
{
    // Fiyat ve teslim süresi kademesi: çıkış ve varış şubesinin şehri/ülkesi karşılaştırılarak bulunur
    public enum RouteScope
    {
        SameCity = 1,        // Aynı şehir içi
        SameCountry = 2,     // Aynı ülke, farklı şehir
        International = 3    // Ülkeler arası
    }
}
