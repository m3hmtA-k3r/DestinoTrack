using DestinoTrack.Business.Options;
using DestinoTrack.DataAccess.Repositories.Cargos;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace DestinoTrack.Business.Services.Cargos
{
    public class TrackCodeGenerator(ICargoRepository _cargoRepository,
                                    IOptions<CargoSettings> _cargoSettings,
                                    IStringLocalizer<SharedResource> _localizer) : ITrackCodeGenerator
    {
        // Çakışma olursa yeniden denenir; bu kadar denemede de bulunamazsa bir sorun var demektir
        private const int MaxAttempts = 10;

        public async Task<string> GenerateAsync()
        {
            var prefix = _cargoSettings.Value.TrackCodePrefix;
            var year = DateTime.UtcNow.Year;

            for (var attempt = 0; attempt < MaxAttempts; attempt++)
            {
                // Rastgele: sıralı numara verilirse günlük kargo adedi dışarıdan sayılabilir
                var number = RandomNumberGenerator.GetInt32(100_000, 1_000_000);
                var trackCode = $"{prefix}-{year}-{number}";

                if (!await _cargoRepository.TrackCodeExistsAsync(trackCode))
                {
                    return trackCode;
                }
            }

            throw new ValidationException(_localizer["TrackCodeGenerationFailed"].Value);
        }
    }
}
