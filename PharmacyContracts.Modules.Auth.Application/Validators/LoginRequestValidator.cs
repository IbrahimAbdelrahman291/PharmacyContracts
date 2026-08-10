using FluentValidation;
using PharmacyContracts.Modules.Auth.Application.DTOs;

namespace PharmacyContracts.Modules.Auth.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
