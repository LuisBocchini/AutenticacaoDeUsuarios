namespace AutenticacaoDeUsuarios.Models
{
    public class ConfigTokenUsuario
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? DataExpiracaoToken { get; set; }
    }
}
