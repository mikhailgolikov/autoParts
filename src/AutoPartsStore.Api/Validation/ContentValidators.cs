using AutoPartsStore.Api.Contracts.News;
using AutoPartsStore.Api.Contracts.Promotions;
using FluentValidation;

namespace AutoPartsStore.Api.Validation;

public class CreateNewsRequestValidator : AbstractValidator<CreateNewsRequest>
{
    public CreateNewsRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.ImagePath).MaximumLength(500);
    }
}

public class UpdateNewsRequestValidator : AbstractValidator<UpdateNewsRequest>
{
    public UpdateNewsRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.ImagePath).MaximumLength(500);
    }
}

public class CreatePromotionRequestValidator : AbstractValidator<CreatePromotionRequest>
{
    public CreatePromotionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.ImagePath).MaximumLength(500);
    }
}

public class UpdatePromotionRequestValidator : AbstractValidator<UpdatePromotionRequest>
{
    public UpdatePromotionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.ImagePath).MaximumLength(500);
    }
}
