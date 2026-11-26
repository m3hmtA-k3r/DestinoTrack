using DestinoTrack.DTO.DTOs.VehicleDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Vehicles
{
    public class CreateVehicleValidator : AbstractValidator<CreateVehicleDto>
    {
        public CreateVehicleValidator(IStringLocalizer<SharedResource> localizer)
        {
            // Plaka düzeni ülkeye göre değişir (TR · MT · BR) — kalıp dayatılmıyor,
            // yalnız boşluk ve uzunluk denetlenir. Tekillik servisin işi 
            RuleFor(x => x.Plate)
                .NotEmpty().WithMessage(localizer["PlateRequired"].Value)
                .MaximumLength(20).WithMessage(localizer["VehiclePlateMaxLength"].Value);

            // Kurye formundaki VehicleTypeRequired "Kurye için..." diyor; araç formunda genel cümle gerekiyor
            RuleFor(x => x.VehicleType)
                .NotNull().WithMessage(localizer["VehicleTypeSelectRequired"].Value)
                .IsInEnum().WithMessage(localizer["VehicleTypeSelectRequired"].Value);

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage(localizer["BranchRequired"].Value);
        }
    }
}
