using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using umfgcloud.apresentacao.identityRepositorio.Contexto;
using umfgcloud.autenticacao.aplicacao.Servicos;
using umfgcloud.autenticacao.dominio.Interfaces.Servicos;

namespace umfgcloud.autenticacao.webapi.Extensions;

internal static class IoCExtensions
{
    internal static void RegistrarServicos(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var conexao = configuration
            .GetConnectionString("MySqlConnection") ?? string.Empty;

        services.AddDbContext<MySqlContexto>(option => 
            option.UseMySQL(conexao), ServiceLifetime.Scoped);

        services
            .AddDefaultIdentity<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MySqlContexto>()
            .AddDefaultTokenProviders();

        services.AddScoped<IAutenticacaoServico, AutenticacaoServico>();
    }
}