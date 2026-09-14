namespace DestinoTrack.Business
{
    // İçi boş kalacak. Bu sınıf sadece çeviri dosyalarına bir "adres" vermek için var.
    // IStringLocalizer<SharedResource> yazdığımızda ASP.NET, bu projenin Resources klasöründe
    // SharedResource.tr.resx / .en.resx / .pt.resx dosyalarını arar.
    //
    // WebUI'dan Business'a taşındı (TALP-20): WebUI Business'ı görebilir ama Business WebUI'yu göremez.
    // Burada durunca hem view'lar hem validator'lar hem servisler aynı çeviri dosyasını kullanabilir.
    public class SharedResource
    {
    }
}
