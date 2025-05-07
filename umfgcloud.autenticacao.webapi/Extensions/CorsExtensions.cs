using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace umfgcloud.autenticacao.webapi.Extensions;

internal static class CorsExtensions
{
    /// <summary>
    /// Adicionar CORS com uma política personalizada
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    internal static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var name = (configuration.GetSection(nameof(CorsOptions)))[nameof(CorsOptions.DefaultPolicyName)]?.Trim() ?? string.Empty;

        services.Configure<CorsOptions>(options => { options.DefaultPolicyName = name; });
        services.AllowAny(name);
    }

    /// <summary>
    /// Configura o pipeline de requisição, utilizando a política definida no arquivo de configuracao
    /// </summary>
    /// <param name="app"></param>
    internal static void UseCorsPolicy(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<CorsOptions>>().Value;

        app.UseAny(options.DefaultPolicyName);
    }

    /// <summary>
    /// Permite qualquer origem (Cuidado com segurança, pode restringir conforme necessário)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="name"></param>
    private static void AllowAny(this IServiceCollection services, string name)
    {
        if (name != "AllowAllOrigins")
            return;

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    private static void UseAny(this IApplicationBuilder app, string name)
    {
        if (name != "AllowAllOrigins")
            return;

        app.UseCors("AllowAllOrigins");
    }
}