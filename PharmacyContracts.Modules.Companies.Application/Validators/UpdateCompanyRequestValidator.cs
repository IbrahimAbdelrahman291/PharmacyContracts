using FluentValidation;
using PharmacyContracts.Modules.Companies.Application.DTOs;

namespace PharmacyContracts.Modules.Companies.Application.Validators;

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequestDto>
{
    public UpdateCompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LocalDiscountPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.ImportedDiscountPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.TaxPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.AdministrativeExpensesPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.ChequeSettlementPeriodInDays).GreaterThan(0);
        RuleFor(x => x.Discount).InclusiveBetween(0, 100);
    }
}