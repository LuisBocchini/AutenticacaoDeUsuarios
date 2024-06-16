using System.ComponentModel.DataAnnotations;

namespace AutenticacaoDeUsuarios.ViewModels
{
    public class DadosViewModel
    {
        [Required]
        public string? Nome { get; set; }
        public string? Foto { get; set; }
    }
}
