
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using umfgcloud.autenticacao.dominio.Classes;

namespace umfgcloud.autenticacao.webapi.Extensions;

internal static class AutenticacaoExtensions
{
    internal static void AddAutenticacao(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions));
        var securityKey = jwtOptions[nameof(JwtOptions.SecurityKey)]?.Trim() ?? string.Empty;
        var issuer = jwtOptions[nameof(JwtOptions.Issuer)]?.Trim() ?? string.Empty;
        var audiance = jwtOptions[nameof(JwtOptions.Audiance)]?.Trim() ?? string.Empty;
        var securityKeyBytes = Encoding.UTF8.GetBytes(securityKey);
        var symmetricSecurityKey = new SymmetricSecurityKey(securityKeyBytes);
        var tokenValidantionsParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = symmetricSecurityKey,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audiance,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(10),
        };

        services.Configure<JwtOptions>(x =>
        {
            x.Issuer = issuer;
            x.Audiance = audiance;
            x.SecurityKey = securityKey;
            x.AccessTokenExpiration = uint.Parse(jwtOptions[nameof(JwtOptions.AccessTokenExpiration)] ?? "0");
            x.SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        });

        services.Configure<IdentityOptions>(x =>
        {
            x.Password.RequireDigit = true;
            x.Password.RequireLowercase = true;
            x.Password.RequireUppercase = true;
            x.Password.RequireNonAlphanumeric = true;
            x.Password.RequiredLength = 6;
        });

        services
            .AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidantionsParameters;
            });
    }
}