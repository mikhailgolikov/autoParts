using AutoPartsStore.Api.Contracts.Attribute;
using AutoPartsStore.Domain.Enums;
using FluentValidation;

namespace AutoPartsStore.Api.Validation;

public class CreateAttributeRequestValidator : AbstractValidator<CreateAttributeRequest>
{
    public CreateAttributeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum().Must(t => Enum.IsDefined(typeof(AttributeType), t));
        RuleFor(x => x.Unit).MaximumLength(32);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class UpdateAttributeRequestValidator : AbstractValidator<UpdateAttributeRequest>
{
    public UpdateAttributeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum().Must(t => Enum.IsDefined(typeof(AttributeType), t));
        RuleFor(x => x.Unit).MaximumLength(32);
    }
}
