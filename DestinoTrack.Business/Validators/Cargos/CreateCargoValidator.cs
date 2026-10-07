using DestinoTrack.DTO.DTOs.CargoDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Cargos
{
    public class CreateCargoValidator : AbstractValidator<CreateCargoDto>
    {
        // Kayıt formuyla aynı telefon kuralı: rakam, boşluk ve baştaki +, 7–20 karakter
        private const string PhonePattern = @"^(?=.{7,20}$)\+?[0-9 ]+$";

        public CreateCargoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.OriginBranchId)
                .NotEmpty().WithMessage(localizer["BranchRequired"].Value);

            RuleFor(x => x.DestinationBranchId)
                .NotEmpty().WithMessage(localizer["BranchRequired"].Value);

            RuleFor(x => x.CargoType)
                .IsInEnum().WithMessage(localizer["CargoTypeInvalid"].Value);

            RuleFor(x => x.PaymentType)
                .IsInEnum().WithMessage(localizer["PaymentTypeInvalid"].Value);

            // Ücret max(ağırlık, desi) üzerinden çıkıyor: sıfır ağırlık fiyatı da sıfırlar
            RuleFor(x => x.Weight)
                .InclusiveBetween(0.01, 1000).WithMessage(localizer["WeightRange"].Value);

            // Ölçü girilmeyebilir (zarf), ama girilince makul olmalı
            RuleFor(x => x.Width).InclusiveBetween(0, 500).WithMessage(localizer["DimensionRange"].Value);
            RuleFor(x => x.Height).InclusiveBetween(0, 500).WithMessage(localizer["DimensionRange"].Value);
            RuleFor(x => x.Length).InclusiveBetween(0, 500).WithMessage(localizer["DimensionRange"].Value);

            // D40 · gönderici
            RuleFor(x => x.SenderName)
                .NotEmpty().WithMessage(localizer["SenderNameRequired"].Value)
                .MaximumLength(100).WithMessage(localizer["NameMaxLength"].Value);

            RuleFor(x => x.SenderPhone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["SenderPhoneRequired"].Value)
                .Matches(PhonePattern).WithMessage(localizer["PhoneInvalid"].Value);

            RuleFor(x => x.SenderAddress)
                .NotEmpty().WithMessage(localizer["SenderAddressRequired"].Value)
                .MaximumLength(500).WithMessage(localizer["AddressMaxLength"].Value);

            // D40 · alıcı
            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage(localizer["ReceiverNameRequired"].Value)
                .MaximumLength(100).WithMessage(localizer["NameMaxLength"].Value);

            RuleFor(x => x.ReceiverPhone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["ReceiverPhoneRequired"].Value)
                .Matches(PhonePattern).WithMessage(localizer["PhoneInvalid"].Value);

            RuleFor(x => x.ReceiverAddress)
                .NotEmpty().WithMessage(localizer["ReceiverAddressRequired"].Value)
                .MaximumLength(500).WithMessage(localizer["AddressMaxLength"].Value);
        }
    }
}
