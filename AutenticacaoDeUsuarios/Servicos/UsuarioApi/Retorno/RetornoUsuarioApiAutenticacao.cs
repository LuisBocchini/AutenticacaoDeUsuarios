namespace AutenticacaoDeUsuarios.Servicos.UsuarioApi.Retorno
{
    public class RetornoUsuarioApiAutenticacao
    {
        public DadosRegistro? Dados { get; set; }
        public string? Mensagem { get; set; }
        public bool Sucesso { get; set; }

    }
    public class DadosRegistro
    {
        public UsuarioApi? Usuario { get; set; }
        public TokenUsuario? TokenUsuario { get; set; }
    }
    public class UsuarioApi
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Nome { get; set; }
        public string? Imagem { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
    }
    public class TokenUsuario
    {
        public string? Token { get; set; }
        public DateTime DataExpiracao { get; set; }
        public string? RefreshToken { get; set; }
    }
}
