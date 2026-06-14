using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoPartsStore.Api.Swagger;

/// <summary>
/// Добавляет требование JWT только к эндпоинтам с [Authorize].
/// Эндпоинты с [AllowAnonymous] остаются без замка.
/// </summary>
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        if (endpointMetadata.OfType<IAllowAnonymous>().Any())
        {
            return;
        }

        var hasAuthorize = endpointMetadata.OfType<IAuthorizeData>().Any();
        if (!hasAuthorize)
        {
            return;
        }

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
}
