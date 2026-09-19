using DestinoTrack.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DestinoTrack.WebUI.Infrastructure
{
    // Configure uygulama açılırken bir kez çalışır; mesajlar ise her istekte o isteğin diliyle üretilir.
    public class LocalizedModelBindingMessages(IStringLocalizer<SharedResource> _localizer) : IConfigureOptions<MvcOptions>
    {
        public void Configure(MvcOptions options)
        {
            var messages = options.ModelBindingMessageProvider;

            // '{0}' değeri geçersiz bu mesajdı
            messages.SetValueIsInvalidAccessor(value => _localizer["Binding_ValueInvalid", value].Value);
            messages.SetValueMustNotBeNullAccessor(value => _localizer["Binding_ValueInvalid", value].Value);
            messages.SetNonPropertyAttemptedValueIsInvalidAccessor(value => _localizer["Binding_ValueInvalid", value].Value);

            // '{0}' değeri {1} alanı için geçersiz
            messages.SetAttemptedValueIsInvalidAccessor((value, field) => _localizer["Binding_ValueInvalidFor", value, field].Value);

            // Girilen değer geçersiz
            messages.SetNonPropertyUnknownValueIsInvalidAccessor(() => _localizer["Binding_UnknownValueInvalid"].Value);
            messages.SetUnknownValueIsInvalidAccessor(field => _localizer["Binding_UnknownValueInvalidFor", field].Value);

            // Sayı olmalı
            messages.SetNonPropertyValueMustBeANumberAccessor(() => _localizer["Binding_MustBeNumber"].Value);
            messages.SetValueMustBeANumberAccessor(field => _localizer["Binding_MustBeNumberFor", field].Value);

            // Değer girilmedi
            messages.SetMissingBindRequiredValueAccessor(field => _localizer["Binding_MissingValueFor", field].Value);
            messages.SetMissingKeyOrValueAccessor(() => _localizer["Binding_ValueRequired"].Value);
            messages.SetMissingRequestBodyRequiredValueAccessor(() => _localizer["Binding_ValueRequired"].Value);
        }
    }
}
