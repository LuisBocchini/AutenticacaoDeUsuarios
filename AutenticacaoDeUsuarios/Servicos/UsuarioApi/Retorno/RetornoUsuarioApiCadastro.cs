namespace AutenticacaoDeUsuarios.Servicos.UsuarioApi.Retorno
{
    public class RetornoUsuarioApiCadastro
    {
        public DadosRegistroCadastro? Dados { get; set; }
        public string? Mensagem { get; set; }
        public bool Sucesso { get; set; }

    }
    public class DadosRegistroCadastro
    {
        public UsuarioCadastro? Usuario { get; set; }
    }
    public class UsuarioCadastro
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Nome { get; set; }
        public string? Senha { get; set; }
        public string? Imagem { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
    }

}
