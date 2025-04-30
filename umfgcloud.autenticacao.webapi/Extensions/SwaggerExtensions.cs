using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace umfgcloud.autenticacao.webapi.Extensions;

internal static class SwaggerExtensions
{
    internal static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments(GetXmlPath());

            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "UMFG Autenticacao API",
                Version = "v1",
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "Cabeçalho de autorização JWT usando o esquema Bearer." +
                              "Digite 'Bearer' [espaço] e, em seguida, seu token na entrada de texto abaixo." +
                              "Exemplo: 'Bearer 12345abcdef",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });
    }

    public static void UseSwaggerUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "UMFG Autenticacao API");
        });        
    }

    private static string GetXmlPath()
        => Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");    
}