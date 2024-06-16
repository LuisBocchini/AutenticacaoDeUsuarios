namespace AutenticacaoDeUsuarios.Models
{
    public class UsuarioAplicacao
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Nome { get; set; }
        public string? Imagem { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? DataExpiracaoToken { get; set; }
        public DateTime? DataCadastro { get; set; }

    }
}
