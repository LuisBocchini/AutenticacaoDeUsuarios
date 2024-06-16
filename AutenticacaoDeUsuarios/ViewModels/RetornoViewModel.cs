namespace AutenticacaoDeUsuarios.ViewModels
{
    public class RetornoViewModel
    {
        public dynamic? Dados { get; private set; }
        public string? Mensagem { get; private set; }
        public bool? Sucesso { get; private set; }
        public RetornoViewModel(dynamic? dados, bool? sucesso, string? mensagem)
        {
            Dados = dados;
            Mensagem = mensagem;
            Sucesso = sucesso;
        }
    }
}
