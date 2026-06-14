using AutoPartsStore.Api.Contracts.Appeals;
using FluentValidation;

namespace AutoPartsStore.Api.Validation;

public class CreateClientQuestionRequestValidator : AbstractValidator<CreateClientQuestionRequest>
{
    public CreateClientQuestionRequestValidator()
    {
        RuleFor(x => x.ManagerComment).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.ContactPhone).MaximumLength(32);
        RuleFor(x => x.ContactEmail).EmailAddress().MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
    }
}

public class CreateSupplierRequestRequestValidator : AbstractValidator<CreateSupplierRequestRequest>
{
    public CreateSupplierRequestRequestValidator()
    {
        RuleFor(x => x.ManagerComment).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ContactPhone).MaximumLength(32);
        RuleFor(x => x.ContactEmail).EmailAddress().MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
    }
}

public class UpdateAppealStatusRequestValidator : AbstractValidator<UpdateAppealStatusRequest>
{
    public UpdateAppealStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class UpdateAppealContactsRequestValidator : AbstractValidator<UpdateAppealContactsRequest>
{
    public UpdateAppealContactsRequestValidator()
    {
        RuleFor(x => x.ContactPhone).NotEmpty().MaximumLength(32);
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(256);
    }
}
