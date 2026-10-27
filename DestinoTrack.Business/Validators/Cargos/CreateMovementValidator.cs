using DestinoTrack.DTO.DTOs.CargoDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Cargos
{
    public class CreateMovementValidator : AbstractValidator<CreateMovementDto>
    {
        public CreateMovementValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.CargoId)
                .NotEmpty().WithMessage(localizer["TrackNotFound"].Value);

            // Burada yalnız "tanımlı bir durum mu" sorulur; geçişin serbest olup olmadığına
            // durum makinesi karar verir (CargoService.ChangeStatusAsync)
            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage(localizer["CargoStatusInvalid"].Value);

            // Açıklama isteğe bağlı, ama serbest metin sınırsız olmasın
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage(localizer["DescriptionMaxLength"].Value);
        }
    }
}
