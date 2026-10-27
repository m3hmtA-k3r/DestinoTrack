namespace DestinoTrack.Entity.Entities.Enums
{
    // Sorunlu kargolar ekranının süzgeci. Bir kargo birden fazlasına girebilir
    // (geciken bir kargonun başarısız denemesi de olabilir), o yüzden bölüm değil çip
    public enum CargoIssue
    {
        FailedAttempt = 1,      // En az bir başarısız teslim denemesi var
        ReturnInProgress = 2,   // İade sürecinde
        Delayed = 3             // Tahmini teslim geçti, hâlâ teslim/iade edilmedi 
    }
}
