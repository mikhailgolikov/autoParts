using Microsoft.OpenApi.Models;

namespace AutoPartsStore.Api.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AutoPartsStore API",
                Version = "v1",
                Description = """
                    API интернет-магазина автозапчастей.

                    Для тестирования ролей:
                    1. Вызовите POST /api/auth/login или /api/auth/register
                    2. Скопируйте token из ответа
                    3. Нажмите Authorize и вставьте токен (без слова Bearer)
                    4. Замок появится только у защищённых эндпоинтов
                    """
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT-токен. Вставьте только token из /api/auth/login (без Bearer).",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoPartsStore API v1");
            options.RoutePrefix = "swagger";
            options.DisplayRequestDuration();
            options.EnablePersistAuthorization();
        });

        return app;
    }
}
