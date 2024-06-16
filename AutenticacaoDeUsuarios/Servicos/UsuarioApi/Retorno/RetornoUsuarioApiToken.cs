namespace AutenticacaoDeUsuarios.Servicos.UsuarioApi.Retorno
{
    public class RetornoUsuarioApiToken
    {
        public Dados? Dados { get; set; }
        public string? Mensagem { get; set; }
        public bool Sucesso { get; set; }
    }
    public class Dados
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? DataExpiracao { get; set; }
    }
}
