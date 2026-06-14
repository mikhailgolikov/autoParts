using AutoPartsStore.Api.Contracts.Certificates;
using FluentValidation;

namespace AutoPartsStore.Api.Validation;

public class CreateCertificateRequestValidator : AbstractValidator<CreateCertificateRequest>
{
    public CreateCertificateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ImagePath).MaximumLength(500);
    }
}

public class UpdateCertificateRequestValidator : AbstractValidator<UpdateCertificateRequest>
{
    public UpdateCertificateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ImagePath).MaximumLength(500);
    }
}
