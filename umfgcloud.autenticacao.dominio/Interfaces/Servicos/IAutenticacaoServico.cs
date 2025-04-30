using umfgcloud.autenticacao.dominio.DTO;

namespace umfgcloud.autenticacao.dominio.Interfaces.Servicos;

public interface IAutenticacaoServico
{
    Task RegistrarUsuarioAsync(UsuarioDTO.RegistrarRequest dto);
    Task<UsuarioDTO.AutenticarResponse> AutenticarUsuarioAsync(UsuarioDTO.AutenticarRequest dto);
}