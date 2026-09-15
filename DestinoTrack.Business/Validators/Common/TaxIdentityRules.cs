using System.Text.RegularExpressions;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Validators.Common
{
    // kimlik / vergi numarasının biçimi ülkeye ve müşteri türüne göre — kontrol hanesi hesaplanmaz.
    public static class TaxIdentityRules
    {
        private static readonly Dictionary<string, Regex> Formats = new()
        {
            ["TR_Individual"] = new Regex(@"^[1-9][0-9]{10}$"),   // TCKN: 11 rakam, 0 ile başlamaz
            ["TR_Corporate"] = new Regex(@"^[0-9]{10}$"),         // VKN: 10 rakam
            ["BR_Individual"] = new Regex(@"^[0-9]{11}$"),        // CPF: 11 rakam
            ["BR_Corporate"] = new Regex(@"^[0-9]{14}$"),         // CNPJ: 14 rakam
            ["MT_Individual"] = new Regex(@"^[0-9]{1,7}[A-Z]$"),  // Kimlik kartı: 1–7 rakam + harf
            ["MT_Corporate"] = new Regex(@"^[0-9]{8}$")           // VAT: 8 rakam
        };

        // Listede olmayan ülkeler
        private static readonly Regex Other = new(@"^[A-Z0-9]{5,20}$");

        // Kullanıcının yazdığı boşluk, nokta, tire ve eğik çizgi atılır, harfler büyütülür:
        // "123.456.789-09" → "12345678909" · "123456m" → "123456M"
        public static string Normalize(string? number) =>
            Regex.Replace(number ?? string.Empty, @"[\s.\-/]", string.Empty).ToUpperInvariant();

        // Numara geçerliyse null, değilse gösterilecek mesajın resx anahtarı döner
        public static string? GetFormatErrorKey(string isoCode, CustomerType customerType, string normalizedNumber)
        {
            var key = $"{isoCode.ToUpperInvariant()}_{customerType}";

            if (Formats.TryGetValue(key, out var format))
            {
                return format.IsMatch(normalizedNumber) ? null : "TaxIdFormat_" + key;
            }

            return Other.IsMatch(normalizedNumber) ? null : "TaxIdFormat_Other";
        }
    }
}
