using DestinoTrack.DTO.DTOs.BranchDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Branches
{
    public class UpdateBranchValidator : AbstractValidator<UpdateBranchDto>
    {
        public UpdateBranchValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(localizer["RecordIdRequired"].Value);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["BranchNameRequired"].Value)
                .MinimumLength(2).WithMessage(localizer["BranchNameMinLength"].Value)
                .MaximumLength(100).WithMessage(localizer["BranchNameMaxLength"].Value);


            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(localizer["BranchCodeRequired"].Value)
                .MaximumLength(20).WithMessage(localizer["BranchCodeMaxLength"].Value)
                .Matches("^[A-Z0-9-]+$").WithMessage(localizer["BranchCodeFormat"].Value);

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage(localizer["CityRequired"].Value);

            RuleFor(x => x.BranchType)
                .IsInEnum().WithMessage(localizer["BranchTypeInvalid"].Value);

            RuleFor(x => x.Capacity)
                .InclusiveBetween(1, 1_000_000).WithMessage(localizer["CapacityRange"].Value);

            RuleFor(x => x.DockCount)
                .InclusiveBetween(0, 1_000).WithMessage(localizer["DockCountRange"].Value);
        }
    }
}
