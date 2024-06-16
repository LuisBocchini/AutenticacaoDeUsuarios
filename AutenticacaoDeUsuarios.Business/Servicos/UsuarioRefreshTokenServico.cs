using AutenticacaoDeUsuarios.Data.Models;
using AutenticacaoDeUsuarios.Data.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoDeUsuarios.Business.Servicos
{
    public class UsuarioRefreshTokenServico
    {
        private readonly UsuarioRefreshTokenRepositorio _usuarioRefreshTokenRepositorio;
        public UsuarioRefreshTokenServico(UsuarioRefreshTokenRepositorio usuarioRefreshTokenRepositorio)
        {
            _usuarioRefreshTokenRepositorio = usuarioRefreshTokenRepositorio;
        }
        public async Task<UsuarioRefreshToken?> ObterRefreshToken(int usuarioId) => await _usuarioRefreshTokenRepositorio.Obter(usuarioId);
        public async Task<UsuarioRefreshToken?> AdicionarRefreshToken(UsuarioRefreshToken usuarioRefreshToken) => await _usuarioRefreshTokenRepositorio.Adicionar(usuarioRefreshToken);
        public async Task<UsuarioRefreshToken?> AtualizarRefreshToken(int usuarioId, string refreshTokenAtual, string novoRefreshToken)
        {
            var refreshTokenUsuario = await _usuarioRefreshTokenRepositorio.Obter(usuarioId);
            if (refreshTokenUsuario?.RefreshToken != refreshTokenAtual)
                return null;

            refreshTokenUsuario.RefreshToken = novoRefreshToken;
            return await _usuarioRefreshTokenRepositorio.Atualizar(refreshTokenUsuario);
        }
        public async Task<UsuarioRefreshToken?> AtualizarOuAdicionarRefreshToken(int usuarioId, string novoRefreshToken)
        {
            var refreshTokenUsuario = await _usuarioRefreshTokenRepositorio.Obter(usuarioId);

            if (refreshTokenUsuario == null)
                return await _usuarioRefreshTokenRepositorio.Adicionar(new UsuarioRefreshToken()
                {
                    UsuarioId = usuarioId,
                    RefreshToken = novoRefreshToken,
                });
            else
            {
                refreshTokenUsuario.RefreshToken = novoRefreshToken;
                return await _usuarioRefreshTokenRepositorio.Atualizar(refreshTokenUsuario);
            }
        }
    }
}
