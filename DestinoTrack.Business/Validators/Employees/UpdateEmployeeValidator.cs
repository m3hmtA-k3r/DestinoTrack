using DestinoTrack.DTO.DTOs.EmployeeDtos;
using DestinoTrack.Entity.Entities.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Employees
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
    {
        public UpdateEmployeeValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(localizer["RecordIdRequired"].Value);

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizer["FirstNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["FirstNameMaxLength"].Value);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizer["LastNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["LastNameMaxLength"].Value);

            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["PhoneRequired"].Value)
                .Matches(@"^(?=.{7,20}$)\+?[0-9 ]+$").WithMessage(localizer["PhoneInvalid"].Value);

            RuleFor(x => x.JobType)
                .IsInEnum().WithMessage(localizer["JobTypeInvalid"].Value);

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage(localizer["BranchRequired"].Value);

            When(x => x.JobType == EmployeeJobType.Courier, () =>
            {
                RuleFor(x => x.VehicleType)
                    .NotNull().WithMessage(localizer["VehicleTypeRequired"].Value)
                    .IsInEnum().WithMessage(localizer["VehicleTypeRequired"].Value);

                RuleFor(x => x.VehiclePlate)
                    .NotEmpty().WithMessage(localizer["VehiclePlateRequired"].Value)
                    .MaximumLength(20).WithMessage(localizer["VehiclePlateMaxLength"].Value);
            });

            RuleFor(x => x.Region)
                .MaximumLength(100).WithMessage(localizer["RegionMaxLength"].Value);
        }
    }
}
