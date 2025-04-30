using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using umfgcloud.autenticacao.dominio.Classes;
using umfgcloud.autenticacao.dominio.DTO;
using umfgcloud.autenticacao.dominio.Interfaces.Servicos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace umfgcloud.autenticacao.aplicacao.Servicos;

public sealed class AutenticacaoServico : IAutenticacaoServico
{
    private readonly JwtSecurityTokenHandler _handler = new();

    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AutenticacaoServico(IOptions<JwtOptions> jwtOptions,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<IdentityUser> signInManager)
    {
        _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    public async Task<UsuarioDTO.AutenticarResponse> AutenticarUsuarioAsync(UsuarioDTO.AutenticarRequest dto)
    {
        var resultado = await _signInManager
            .PasswordSignInAsync(dto.Email, dto.Senha, false, true);

        if (resultado.Succeeded)
            return await GetResponseAsync(dto.Email);

        if (resultado.IsLockedOut)
            throw new InvalidOperationException("Conta bloqueada!");
        else if (resultado.IsNotAllowed)
            throw new InvalidOperationException("Conta sem permissão de login!");
        else if (resultado.RequiresTwoFactor)
            throw new InvalidOperationException("É necessário confirmar login com a autenticação " +
                "de dois fatores!");

        throw new InvalidOperationException("Usuário ou senha incorretos!");
    }

    private async Task<UsuarioDTO.AutenticarResponse> GetResponseAsync(string email)
    {        
        var dataAtual = DateTime.Now;
        var dataExpiracao = dataAtual.AddHours(_jwtOptions.AccessTokenExpiration);

        var usuario = await _userManager.FindByEmailAsync(email)
            ?? throw new InvalidOperationException("Usuário não encontrado!");

        var claims = new List<Claim>
        {
            //subject
            //Descrição: Representa o assunto do token. Normalmente, é o usuário ao qual o token se refere.
            //Exemplo de uso: Usado para armazenar o ID do usuário ou qualquer identificador único
            //  que possa associar o token ao usuário.
            new (JwtRegisteredClaimNames.Sub, usuario.Id ?? string.Empty),

            //Descrição: Representa o endereço de e-mail do usuário.
            //Exemplo de uso: Se você estiver usando um sistema de autenticação que exija
            //  o e-mail do usuário, pode armazenar o e-mail aqui.
            new (JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty),

            //Descrição: Representa o identificador de nome
            //  (pode ser usado para o identificador único de um usuário ou algo semelhante).
            //Exemplo de uso: Normalmente, é utilizado como um identificador
            //  único(como um ID de login ou o nome do usuário),
            //  mas sua semântica exata pode variar de acordo com o sistema de autenticação.
            new (JwtRegisteredClaimNames.NameId, usuario.UserName ?? string.Empty),

            //JWT ID
            //Descrição: Representa um identificador único para o token. Esse valor é
            //  útil para garantir que o token é único e pode ser usado para
            //  prevenir ataques de replay (quando um token é capturado e usado novamente).
            //Exemplo de uso: Utilizado para gerar um ID único para o token,
            //  de modo a identificar um token específico de maneira única.
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            //Not Before
            //Descrição: Representa a data e hora a partir da qual o token é válido.
            //  O token não será válido antes dessa data/ hora.
            //Exemplo de uso: Pode ser usado para garantir que o token não seja aceito
            //  antes de um determinado horário. Isso é útil para impedir que o token seja usado antes do início da sua validade.
            new (JwtRegisteredClaimNames.Nbf, dataAtual.ToString()),

            //Issued At
            //Descrição: Representa a data e hora em que o token foi emitido.
            //Exemplo de uso: O token terá essa claim, indicando a data de emissão.Isso é útil para determinar a "idade" do token.
            new (JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
        };

        var securityTokenDecriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audiance,
            Expires = dataExpiracao,
            NotBefore = dataAtual,
            SigningCredentials = _jwtOptions.SigningCredentials,
        };

        var jwt = _handler.CreateToken(securityTokenDecriptor);
        var token = _handler.WriteToken(jwt);

        return new UsuarioDTO.AutenticarResponse()
        {
            Token = token,
            DataExpiracao = dataExpiracao,
        };
    }

    public async Task RegistrarUsuarioAsync(UsuarioDTO.RegistrarRequest dto)
    {
        var usuario = new IdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
        };

        var resultado = await _userManager.CreateAsync(usuario, dto.Senha)
            ?? throw new InvalidOperationException("Falha ao criar usuário!");

        if (!resultado.Succeeded)
            throw new InvalidOperationException(GetMensagensErro(resultado));

        await _userManager.SetLockoutEnabledAsync(usuario, false);
    }

    private static string? GetMensagensErro(IdentityResult resultado)
        => string.Join(",", resultado.Errors.Select(x => x.Description));
}