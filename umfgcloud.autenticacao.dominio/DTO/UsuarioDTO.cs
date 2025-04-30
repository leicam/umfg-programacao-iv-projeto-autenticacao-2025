using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace umfgcloud.autenticacao.dominio.DTO;

public sealed class UsuarioDTO
{
    public sealed class RegistrarRequest
    {
        [JsonPropertyName("email")]
        [Required(ErrorMessage = "O campo {0} é obrigatório! Verifique.")]
        [EmailAddress(ErrorMessage = "O campo {0} é inválido! Verifique.")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("senha")]
        [Required(ErrorMessage = "O campo {0} é obrigatório! Verifique.")]
        [StringLength(50, 
            ErrorMessage = "O campo {0} deve ter entre {1} e {2} caracteres!", MinimumLength = 6)]
        public string Senha { get; set; } = string.Empty;

        [JsonPropertyName("senhaConfirmada")]
        [Required(ErrorMessage = "O campo {0} é obrigatório! Verifique.")]
        [Compare(nameof(Senha), ErrorMessage = "As senhas devem ser iguais! Verifique.")]
        public string SenhaConfirmada { get; set; } = string.Empty;
    }

    public sealed class AutenticarRequest
    {
        [JsonPropertyName("email")]
        [Required(ErrorMessage = "O campo {0} é obrigatório! Verifique.")]
        [EmailAddress(ErrorMessage = "O campo {0} é inválido! Verifique.")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("senha")]
        [Required(ErrorMessage = "O campo {0} é obrigatório! Verifique.")]        
        public string Senha { get; set; } = string.Empty;
    }

    public sealed class AutenticarResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("dataExpiracao")]
        public DateTime DataExpiracao { get; set; } = DateTime.MinValue;
    }
}