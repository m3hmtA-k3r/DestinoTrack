using DestinoTrack.DataAccess.Repositories.Customers;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace DestinoTrack.Business.Services.Customers
{
    // Müşteri kodu TEK yerde üretilir: kayıt formu da admin ekranı da buradan geçer.
    // Önceden AccountService'in private metoduydu; admin ekranı gelince ikinci bir kopya
    // doğmasın diye ortak yere alındı (TrackCodeGenerator kalıbı)
    public class CustomerCodeGenerator(ICustomerRepository _customerRepository,
                                       IStringLocalizer<SharedResource> _localizer) : ICustomerCodeGenerator
    {
        // Çakışma olursa yeniden denenir; bu kadar denemede de bulunamazsa bir sorun var demektir
        private const int MaxAttempts = 10;

        public async Task<string> GenerateAsync()
        {
            for (var attempt = 0; attempt < MaxAttempts; attempt++)
            {
                // Rastgele: sıralı numara verilirse kayıtlı müşteri sayısı dışarıdan tahmin edilebilir
                var number = RandomNumberGenerator.GetInt32(10_000, 100_000);
                var code = $"CUK-{DateTime.UtcNow:yyMMdd}-{number}";

                if (!await _customerRepository.CodeExistsAsync(code))
                {
                    return code;
                }
            }

            throw new ValidationException(_localizer["CustomerCodeGenerationFailed"].Value);
        }
    }
}
