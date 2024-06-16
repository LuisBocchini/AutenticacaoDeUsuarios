using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoDeUsuarios.Business.Mensagens
{
    public class Registro
    {
        public const string UsuarioInvalido = "Usuário inválido";
        public const string UsuarioInexistente = "Usuário não existe";
        public const string UsuarioCadastrado = "Usuário cadastrado com sucesso, é necessário confirmar o e-mail";
        public const string UsuarioEditado = "Usuário editado com sucesso";
        public const string UsuarioExistente = "O usuário já existe";
        public const string UsuarioAutenticado = "Usuário autenticado com sucesso";
        public const string UsuarioInativo = "O usuário está inativo, é necessário confirmar o e-mail. Um e-mail de confirmação foi enviado";
        public const string ErroCadastro = "Erro ao cadastrar usuário";
        public const string ErroEdicao = "Erro ao editar usuário";
        public const string InformacoesInvalidas = "Informações inválidas para realizar o login";
        public const string SenhaInvalidaEdicao = "A senha está inválida para realizar a edição";
        public const string EmailInvalido = "O Email está inválido";
        public const string ErroAtualizarSenha = "Erro ao atualizar a senha";
        public const string SenhaAtualizada = "Senha atualizada";
        public const string SenhaAnteriorInvalida = "A senha anterior está inválida";
        public const string TokenExpirado = "Token expirado";
        public const string ContaAtivada = "Conta ativada com sucesso";
        public const string ErroAoAtivarConta = "Erro ao ativar conta";
        public const string EmailEnviado = "Email enviado com sucesso";
        public const string SenhaIncorreta = "Senha incorreta";
    }
}
