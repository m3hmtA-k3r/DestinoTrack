using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Infrastructure
{
    public static class CargoStatusStyle
    {
        public static string ChipClass(this CargoStatus status) => status switch
        {
            CargoStatus.Delivered => "chip-active",
            CargoStatus.DeliveryFailed or CargoStatus.ReturnInProgress or CargoStatus.ReturnedToSender => "chip-error",
            _ => "chip-info"
        };
    }
}
