using Microsoft.IdentityModel.Tokens;

namespace umfgcloud.autenticacao.dominio.Classes;

public sealed class JwtOptions
{
    public string SecurityKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audiance { get; set; } = string.Empty;
    public uint AccessTokenExpiration { get; set; } = 0;
    public SigningCredentials? SigningCredentials { get; set; }
}