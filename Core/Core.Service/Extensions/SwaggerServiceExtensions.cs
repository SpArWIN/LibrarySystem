using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Service.Extensions;

/// <summary>
/// Расширене на Swagger.
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Добавить сваггер с xml.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="assemblies">Сборки.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddSwaggerWithXml(this IServiceCollection services,
        params  Assembly[] assemblies)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Library System API",
                Version = "V1",
                Description = "Документация"
            });
            c.IncludeXmlFromAssemblies(assemblies);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT: вставьте только access token (префикс Bearer добавится автоматически).",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference() { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    private static void IncludeXmlFromAssemblies(this SwaggerGenOptions swaggerGenOptions, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies.Distinct())
        {
            var xmlFile = $"{assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                swaggerGenOptions.IncludeXmlComments(xmlPath);
            }
        }
    }
    
    /// <summary>
    /// Добавить SwaggerUI.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
    /// <returns>.</returns>
    public static IApplicationBuilder UseSwaggerWithUi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library System API");
            c.RoutePrefix = string.Empty;
        });
        return app;
    }
}