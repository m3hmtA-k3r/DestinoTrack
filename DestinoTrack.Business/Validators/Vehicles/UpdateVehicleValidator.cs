using DestinoTrack.DTO.DTOs.VehicleDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Vehicles
{
    public class UpdateVehicleValidator : AbstractValidator<UpdateVehicleDto>
    {
        public UpdateVehicleValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Plate)
                .NotEmpty().WithMessage(localizer["PlateRequired"].Value)
                .MaximumLength(20).WithMessage(localizer["VehiclePlateMaxLength"].Value);

            RuleFor(x => x.VehicleType)
                .NotNull().WithMessage(localizer["VehicleTypeSelectRequired"].Value)
                .IsInEnum().WithMessage(localizer["VehicleTypeSelectRequired"].Value);

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage(localizer["BranchRequired"].Value);
        }
    }
}
