namespace DestinoTrack.Entity.Entities.Enums
{
    public enum CargoType
    {
        Standard = 1,   // Standart gönderi
        Urgent = 2,     // Acil — öncelikli taşınır
        Fragile = 3,    // Kırılabilir — özel paketleme
        Heavy = 4,      // Ağır yük — ek ücret
        Document = 5,   // Dosya / zarf — sabit tarife
        ColdChain = 6,  // Soğuk zincir — gıda, ilaç
        Valuable = 7,   // Değerli — sigortalı gönderi
        Oversized = 8,  // Büyük hacimli — hacim üzerinden ücret
    }
}
