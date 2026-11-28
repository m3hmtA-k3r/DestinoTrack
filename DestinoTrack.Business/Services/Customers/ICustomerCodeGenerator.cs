namespace DestinoTrack.Business.Services.Customers
{
    public interface ICustomerCodeGenerator
    {
        // CUK-{yyMMdd}-{5 rakam} — veritabanında benzersiz
        Task<string> GenerateAsync();
    }
}
