using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DestinoTrack.Entity.Entities.Enums
{
    public enum PaymentType
    {
        SenderPays = 1,     // Gönderici ödemeli
        ReceiverPays = 2    // Alıcı ödemeli — kapıda tahsilat
    }
}
