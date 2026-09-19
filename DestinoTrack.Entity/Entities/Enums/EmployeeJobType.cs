namespace DestinoTrack.Entity.Entities.Enums
{
    public enum EmployeeJobType
    {
        Courier = 1,          // Kurye — dağıtım yapar, araç bilgileri dolu olur
        BranchStaff = 2,      // Şube personeli — kabul, teslim, gişe
        WarehouseStaff = 3,   // Depo / transfer merkezi görevlisi — yükleme, aktarma
        Driver = 4,           // Şoför — şubeler arası sevkiyat
    }
}
