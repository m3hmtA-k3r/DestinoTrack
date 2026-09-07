namespace DestinoTrack.Entity.Entities.Enums
{
    public enum PaymentMethod
    {
        Cash = 1,           // Nakit
        CreditCard = 2,     // Kredi kartı
        BankTransfer = 3,   // Havale / EFT
        CorporateInvoice = 4 // Kurumsal fatura — sonra tahsil edilir
    }
}
