using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Infrastructure
{
    // MVC, boş olamayan değer tiplerini (int · decimal · enum · bool · Guid) zorunlu sayar ama
    // bunun için model bilgisine bir [Required] koymaz: tarayıcı doğrulamasını kurarken mesajsız bir tane üretir
    // ve mesaj İngilizce gelir. Çözüm: özniteliği resx anahtarıyla biz ekliyoruz, MVC kendi kopyasını üretmiyor.
    public class LocalizedRequiredMetadataProvider : IValidationMetadataProvider
    {
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            // Yalnızca DTO özellikleri; tür ve parametre bilgisi ilgilendirmiyor
            if (context.Key.MetadataKind != ModelMetadataKind.Property)
            {
                return;
            }

            var type = context.Key.ModelType;
            var isNonNullableValueType = type.IsValueType && Nullable.GetUnderlyingType(type) is null;
            if (!isNonNullableValueType)
            {
                return;
            }

            // Guid: açılır listelerin FluentValidation mesajı daha açık ("Şehir seçilmelidir.")
            // bool: onay kutusu her zaman bir değer gönderir, mesaj görünmez
            if (type == typeof(Guid) || type == typeof(bool))
            {
                return;
            }

            // Kendi yazdığımız [Required] varsa dokunulmaz
            if (context.ValidationMetadata.ValidatorMetadata.OfType<RequiredAttribute>().Any())
            {
                return;
            }

            // Mesaj bir resx anahtarı; çeviriyi AddDataAnnotationsLocalization yapıyor
            context.ValidationMetadata.ValidatorMetadata.Add(new RequiredAttribute { ErrorMessage = "FieldRequired" });
        }
    }
}
