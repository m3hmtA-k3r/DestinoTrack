namespace DestinoTrack.Business.Services.Cargos
{
    public interface ITrackCodeGenerator
    {
        //{Önek}-{Yıl}-{6 rakam} — veritabanında benzersiz
        Task<string> GenerateAsync();
    }
}
