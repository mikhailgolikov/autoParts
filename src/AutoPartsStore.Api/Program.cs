using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Services;
using AutoPartsStore.Api.Swagger;
using AutoPartsStore.Api.Validation;
using AutoPartsStore.Infrastructure;
using AutoPartsStore.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AppealService>();
builder.Services.AddScoped<SiteContentService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<AttributeService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<PromotionService>();
builder.Services.AddScoped<CertificateService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
    await CompanySeeder.SeedAsync(dbContext);
    await AdminSeeder.SeedAsync(dbContext, app.Configuration);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
