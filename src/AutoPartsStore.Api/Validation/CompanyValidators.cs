using AutoPartsStore.Api.Contracts.Companies;
using AutoPartsStore.Api.Contracts.Contacts;
using FluentValidation;

namespace AutoPartsStore.Api.Validation;

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Address).MaximumLength(500);
    }
}

public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
{
    public CreateContactRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

public class UpdateContactRequestValidator : AbstractValidator<UpdateContactRequest>
{
    public UpdateContactRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
