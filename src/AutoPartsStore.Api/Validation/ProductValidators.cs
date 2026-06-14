using AutoPartsStore.Api.Contracts.Product;
using FluentValidation;

namespace AutoPartsStore.Api.Validation;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Article).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.ImagePath).MaximumLength(500);
        RuleFor(x => x.BrandId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleForEach(x => x.AttributeValues).ChildRules(v =>
        {
            v.RuleFor(a => a.AttributeId).NotEmpty();
            v.RuleFor(a => a.Value).NotEmpty().MaximumLength(500);
        });
    }
}

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Article).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.ImagePath).MaximumLength(500);
        RuleFor(x => x.BrandId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleForEach(x => x.AttributeValues).ChildRules(v =>
        {
            v.RuleFor(a => a.AttributeId).NotEmpty();
            v.RuleFor(a => a.Value).NotEmpty().MaximumLength(500);
        });
    }
}

public class UpdateProductStockRequestValidator : AbstractValidator<UpdateProductStockRequest>
{
    public UpdateProductStockRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductPriceRequestValidator : AbstractValidator<UpdateProductPriceRequest>
{
    public UpdateProductPriceRequestValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
