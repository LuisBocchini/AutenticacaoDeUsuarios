using System.ComponentModel.DataAnnotations;

namespace AutenticacaoDeUsuarios.API.ViewModels
{
    public class CadastroViewModel
    {
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string? Email { get; set; }
        public string? Nome { get; set; }
        [Required(ErrorMessage = "A Senha é obrigatória")]
        public string? Senha { get; set; }
        public string? Imagem { get; set; }
        public string? UrlRedirecionamento { get; set; }
        public DateTime DataCadastro => DateTime.Now;
    }
}
