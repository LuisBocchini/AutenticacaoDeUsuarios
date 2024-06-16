using AutenticacaoDeUsuarios.API.ViewModels;
using AutenticacaoDeUsuarios.Business.Mensagens;
using AutenticacaoDeUsuarios.Business.Models;
using AutenticacaoDeUsuarios.Business.Servicos;
using AutenticacaoDeUsuarios.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDeUsuarios.API.Controllers
{
    [Route("api/senha")]
    [ApiController]
    public class SenhaController : ControllerBase
    {
        private readonly UsuarioServico _usuarioServico;
        private readonly EmailServico _emailServico;
        public SenhaController(UsuarioServico usuarioServico, EmailServico emailServico)
        {
            _usuarioServico = usuarioServico;
            _emailServico = emailServico;
        }

        [HttpPost("enviarEmailRedefinicaoSenha")]
        [AllowAnonymous]
        public async Task<IActionResult> EnviarEmailRedefinicaoSenha(EnvioEmailViewModel model)
        {
            try
            {
                var usuario = await _usuarioServico.ObterUsuarioPorEmail(model.Email);
                if (usuario == null)
                    return BadRequest(new RetornoViewModel(null, false, Registro.UsuarioInexistente));

                var token = TokenServico.GerarToken(usuario, 10);

                _emailServico.EnviarEmail("Redefinição de Senha", $"Olá {usuario.Nome}, você solicitou uma redefinição de senha." +
                    $"Clique no link abaixo para atualizar sua senha <br><br>" +
                    $"<strong>Redefinição de Senha:<strong> https://localhost:7053/api/senha/redirecionar?token={token}&urlRedirecionamento={model.UrlRedirecionamento}", usuario.Email);

                return Ok(new RetornoViewModel(null, true, Registro.EmailEnviado));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

        [HttpGet("redirecionar")]
        [AllowAnonymous]
        public IActionResult Redirecionar(string token, string urlRedirecionamento)
        {
            try
            {
                var tokenUsuario = TokenServico.ObterClaimsToken(token);
                var email = tokenUsuario?.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
                return Redirect($"{urlRedirecionamento}?token={token}&email={email}");
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

        [HttpPut("redefinirSenha")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
        {
            try
            {
                var claimsUsuario = TokenServico.ObterClaimsToken(model.Token);
                var email = claimsUsuario?.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
                var dataExpiracao = TokenServico.ObterDataExpiracaoToken(model.Token);

                if (dataExpiracao < DateTime.Now)
                    return BadRequest(new RetornoViewModel(null, false, Registro.TokenExpirado));

                var usuario = await _usuarioServico.ObterUsuarioPorEmail(email);

                var usuarioEdicao = await _usuarioServico.RedefinirSenha(usuario.Email, model.NovaSenha);
                if (!usuarioEdicao.sucesso)
                    return BadRequest(new RetornoViewModel(null, false, usuarioEdicao.mensagem != null ? usuarioEdicao.mensagem : Registro.ErroAtualizarSenha));

                return Ok(new RetornoViewModel(null, true, Registro.SenhaAtualizada));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

        [HttpPut("atualizarSenha")]
        [Authorize]
        public async Task<IActionResult> AtualizarSenha([FromBody] EditarSenhaViewModel model)
        {
            try
            {
                var usuarioEdicao = await _usuarioServico.AtualizarSenha(model.Email, model.SenhaAtual, model.NovaSenha);
                if (!usuarioEdicao.sucesso)
                {
                    return BadRequest(new RetornoViewModel(null, false, usuarioEdicao.mensagem));
                }
                return Ok(new RetornoViewModel(null, true, usuarioEdicao.mensagem));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }
    }
}
