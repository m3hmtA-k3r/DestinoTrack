namespace DestinoTrack.Entity.Entities.Enums
{
    public enum DeliveryFailureReason
    {    // teslim denemesi neden başarısız oldu — her başarısız deneme DeliveryException satırına bu gerekçeyle yazılır
        AddressNotFound = 1,        
        RecipientUnavailable = 2,   
        RecipientRefused = 3,       
        Damaged = 4,    
        Other = 5   
    }
}
