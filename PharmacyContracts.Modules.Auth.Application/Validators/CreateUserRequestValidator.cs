using FluentValidation;
using PharmacyContracts.Modules.Auth.Application.DTOs;

namespace PharmacyContracts.Modules.Auth.Application.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequestDto>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(r => r is "SuperAdmin" or "Pharmacy")
                .WithMessage("Role must be either 'SuperAdmin' or 'Pharmacy'.");
            RuleFor(x => x.PharmacyName)
                .NotEmpty()
                .When(x => x.Role == "Pharmacy")
                .WithMessage("PharmacyName is required when role is Pharmacy.");
        }
    }
}
