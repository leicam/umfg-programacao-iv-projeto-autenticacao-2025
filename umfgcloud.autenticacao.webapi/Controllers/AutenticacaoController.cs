using Microsoft.AspNetCore.Mvc;
using umfgcloud.autenticacao.dominio.DTO;
using umfgcloud.autenticacao.dominio.Interfaces.Servicos;

namespace umfgcloud.autenticacao.webapi.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public sealed class AutenticacaoController : ControllerBase
    {
        private readonly IAutenticacaoServico _servico;

        public AutenticacaoController(IAutenticacaoServico servico)
            => _servico = servico ?? throw new ArgumentNullException(nameof(servico));

        /// <summary>
        /// Cadastra um usuário
        /// </summary>
        /// <param name="dto">Dados de cadastro do usuário, todos são obrigatórios</param>
        /// <returns></returns>
        [HttpPost("registar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarUsuarioAsync(
            [FromBody] UsuarioDTO.RegistrarRequest dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(GetMenssagensErro());

                await _servico.RegistrarUsuarioAsync(dto);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }   
        }

        /// <summary>
        /// Cadastra um usuário
        /// </summary>
        /// <param name="dto">Dados de cadastro do usuário, todos são obrigatórios</param>
        /// <returns></returns>
        [HttpPost("autenticar")]
        [ProducesResponseType(typeof(UsuarioDTO.AutenticarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AutenticarUsuarioAsync(
            [FromBody] UsuarioDTO.AutenticarRequest dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(GetMenssagensErro());

                return Ok(await _servico.AutenticarUsuarioAsync(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetMenssagensErro()
            => string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors));
    }
}
