using AutenticacaoDeUsuarios.Business.Mensagens;
using AutenticacaoDeUsuarios.Business.Models;
using AutenticacaoDeUsuarios.Data.Models;
using AutenticacaoDeUsuarios.Data.Repositorio;
using Microsoft.AspNetCore.Identity;


namespace AutenticacaoDeUsuarios.Business.Servicos
{
    public class UsuarioServico
    {
        private readonly UsuarioRepositorio _usuarioRepositorio;
        public UsuarioServico(UsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }
        public async Task<Usuario?> RegistrarUsuario(Usuario usuario)
        {
            return await _usuarioRepositorio.Adicionar(usuario);
        }
        public async Task<Usuario?> ObterUsuarioPorEmail(string? email)
        {
            if (email == null)
                return null;

            var usuario = await _usuarioRepositorio.Obter(email);
            return usuario;
        }
        public async Task<(Usuario? usuario, string? mensagem)> EditarUsuario(string? email, EdicaoUsuarioModel model)
        {
            if (email == null)
                return (null, Registro.EmailInvalido);

            var usuario = await _usuarioRepositorio.Obter(email);

            if (usuario == null)
                return (null, Registro.UsuarioInvalido);

            if (!string.IsNullOrEmpty(model.Nome))
                usuario.Nome = model.Nome;
            if (!string.IsNullOrEmpty(model.Imagem))
                usuario.Imagem = model.Imagem;

            var usuarioEdicao = await _usuarioRepositorio.Atualizar(usuario);

            if (usuarioEdicao != null)
                return (usuarioEdicao, null);
            else
                return (null, null);

        }
        public async Task<bool> AtivarUsuario(string? email)
        {
            var usuario = await _usuarioRepositorio.Obter(email);
            usuario.Ativo = true;

            var usuarioAtivo = await _usuarioRepositorio.Atualizar(usuario);

            if (usuarioAtivo != null)
                return true;
            else
                return false;
        }
        public async Task<(bool sucesso, string? mensagem)> AtualizarSenha(string? email, string? senhaAtual, string? novaSenha)
        {
            if (email == null)
                return (false, null);

            var usuario = await _usuarioRepositorio.Obter(email);
            var hashSenhaAtual = new PasswordHasher<Usuario>().VerifyHashedPassword(usuario, usuario.Senha, senhaAtual);

            if (hashSenhaAtual == PasswordVerificationResult.Success)
            {
                usuario.Senha = new PasswordHasher<Usuario>().HashPassword(usuario, novaSenha);
                var edicaoUsuario = await _usuarioRepositorio.Atualizar(usuario);

                if (edicaoUsuario != null)
                    return (true, Registro.SenhaAtualizada);

                return (false, Registro.ErroAtualizarSenha);
            }

            return (false, Registro.SenhaAnteriorInvalida);
        }
        public async Task<(bool sucesso, string? mensagem)> RedefinirSenha(string? email, string? novaSenha)
        {
            if (email == null)
                return (false, null);

            var usuario = await _usuarioRepositorio.Obter(email);
            usuario.Senha = new PasswordHasher<Usuario>().HashPassword(usuario, novaSenha);

            var usuarioEdicao = await _usuarioRepositorio.Atualizar(usuario);
            if (usuarioEdicao is null)
                return (false, Registro.ErroAtualizarSenha);

            return (true, Registro.SenhaAtualizada);
        }
    }
}
