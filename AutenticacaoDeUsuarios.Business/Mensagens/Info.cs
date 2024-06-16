using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoDeUsuarios.Business.Mensagens
{
    public class Info
    {
        public const string ErroInterno = "Erro interno no servidor";
        public const string ErroRefreshToken = "Erro ao atualizar refresh token";
        public const string ErroAutenticacao = "Erro ao fazer autenticação";
        public const string ErroCadastro= "Erro ao cadastrar";
        public const string ErroConsulta= "Erro ao consultar usuário";
        public const string Sucesso= "Sucesso";
    }
}
