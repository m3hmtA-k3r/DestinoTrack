using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Validators.Common;
using DestinoTrack.DTO.DTOs.EmployeeDtos;
using DestinoTrack.Entity.Entities.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DestinoTrack.Business.Validators.Employees
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        // IdentityOptions: şifre kuralları Program.cs'ten okunur — kural iki yerde yazılmaz (CreateUserValidator ile aynı)
        public CreateEmployeeValidator(IStringLocalizer<SharedResource> localizer, IOptions<IdentityOptions> identityOptions)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizer["FirstNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["FirstNameMaxLength"].Value);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizer["LastNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["LastNameMaxLength"].Value);

            // Kayıt formuyla aynı telefon kuralı: rakam, boşluk ve baştaki +, 7–20 karakter
            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["PhoneRequired"].Value)
                .Matches(@"^(?=.{7,20}$)\+?[0-9 ]+$").WithMessage(localizer["PhoneInvalid"].Value);

            RuleFor(x => x.JobType)
                .IsInEnum().WithMessage(localizer["JobTypeInvalid"].Value);

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage(localizer["BranchRequired"].Value);

            // Kurye alanları: yalnızca kuryede zorunlu
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

            // Giriş hesabı: yalnızca "hesap aç" işaretliyse
            When(x => x.CreateAccount, () =>
            {
                RuleFor(x => x.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(localizer["EmailRequired"].Value)
                    .EmailAddress().WithMessage(localizer["EmailInvalid"].Value)
                    .MaximumLength(256).WithMessage(localizer["EmailMaxLength"].Value);

                RuleFor(x => x.Password)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(localizer["PasswordRequired"].Value)
                    .PasswordPolicy(identityOptions.Value.Password, localizer);

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.Password).WithMessage(localizer["ConfirmPasswordMismatch"].Value);

                // rol formda seçilir, yalnızca iki personel rolü
                RuleFor(x => x.AccountRole)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(localizer["RoleRequired"].Value)
                    .Must(role => role == RoleNames.Personel || role == RoleNames.Courier).WithMessage(localizer["AccountRoleInvalid"].Value);
            });
        }
    }
}
