using AutenticacaoDeUsuarios.API.ViewModels;
using AutenticacaoDeUsuarios.Business.Models;
using AutenticacaoDeUsuarios.Business.Servicos;
using AutenticacaoDeUsuarios.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutenticacaoDeUsuarios.Business.Mensagens;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace AutenticacaoDeUsuarios.API.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioServico _usuarioServico;
        private readonly UsuarioRefreshTokenServico _usuarioRefreshTokenServico;
        private readonly EmailServico _emailServico;
        public UsuarioController(UsuarioServico usuarioServico, EmailServico emailServico, UsuarioRefreshTokenServico usuarioRefreshTokenServico)
        {
            _usuarioServico = usuarioServico;
            _emailServico = emailServico;
            _usuarioRefreshTokenServico = usuarioRefreshTokenServico;
        }

        [HttpPost("autenticar")]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar([FromBody] LoginViewModel model)
        {
            try
            {
                var usuario = await _usuarioServico.ObterUsuarioPorEmail(model.Email);

                if (usuario == null)
                    return NotFound(new RetornoViewModel(null, false, Registro.UsuarioInexistente));

                if (usuario.Ativo == false)
                {
                    var reenvioEmail = new EnvioEmailViewModel()
                    {
                        Email = usuario.Email,
                        UrlRedirecionamento = "https://localhost:7066"
                    };
                    
                    await ReenviarEmailAtivacaoConta(reenvioEmail);
                    return BadRequest(new RetornoViewModel(null, false, Registro.UsuarioInativo));
                }

                var validaSenha = new PasswordHasher<Usuario>().VerifyHashedPassword(usuario, usuario.Senha, model.Senha);

                if (validaSenha == PasswordVerificationResult.Success)
                {
                    var usuarioRefreshToken = await _usuarioRefreshTokenServico.AtualizarOuAdicionarRefreshToken(usuario.Id, TokenServico.GerarRefreshToken());
                    if (usuarioRefreshToken == null)
                        return BadRequest(new RetornoViewModel(null, false, "Erro ao atualizar refresh token"));

                    usuario.Senha = null;
                    var token = TokenServico.GerarToken(usuario);
                    var dadosRetorno = new
                    {
                        Usuario = usuario,
                        TokenUsuario = new
                        {
                            Token = token,
                            DataExpiracao = TokenServico.ObterDataExpiracaoToken(token),
                            RefreshToken = usuarioRefreshToken?.RefreshToken
                        }
                    };
                    return Ok(new RetornoViewModel(dadosRetorno, true, Registro.UsuarioAutenticado));
                }
                else
                    return BadRequest(new RetornoViewModel(null, false, Registro.SenhaIncorreta));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }


        [HttpPost("cadastrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Cadastrar([FromBody] CadastroViewModel model)
        {
            try
            {
                var usuarioExistente = await _usuarioServico.ObterUsuarioPorEmail(model.Email);
                if (usuarioExistente != null)
                    return BadRequest(new RetornoViewModel(null, false, Registro.UsuarioExistente));

                var usuario = new Usuario()
                {
                    Nome = model.Nome,
                    Email = model.Email,
                    Imagem = model.Imagem,
                    DataCadastro = model.DataCadastro,
                    Ativo = false
                };

                var hashSenha = new PasswordHasher<Usuario>().HashPassword(usuario, model.Senha);
                usuario.Senha = hashSenha;

                var usuarioRegistro = await _usuarioServico.RegistrarUsuario(usuario);
                if (usuarioRegistro != null)
                {
                    usuarioRegistro.Senha = null;
                    var tokenUsuario = TokenServico.GerarToken(usuario, 10);

                    _emailServico.EnviarEmail("Sistema de Autenticação De Usuários", 
                        $"<strong> Bem vindo {usuario.Nome}!</strong> <br> " +
                        $"Você acabou de se cadastrar em nossa plataforma. " +
                        $"Clique no link abaixo para confirmar seu e-mail e validar sua conta <br> <br> " +
                        $"<strong>Confirmação:<strong> https://localhost:7053/api/usuario/ativarConta?token={tokenUsuario}&urlRedirecionamento={model.UrlRedirecionamento}", 
                        usuario.Email);

                    var usuarioCadastro = new
                    {
                        Usuario = usuario,
                    };

                    return Ok(new RetornoViewModel(usuarioCadastro, true, Registro.UsuarioCadastrado));
                }
                else
                    return BadRequest(new RetornoViewModel(null, false, Registro.ErroCadastro));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

        [HttpPut("editar")]
        [Authorize]
        public async Task<IActionResult> Editar([FromBody] EditarViewModel model)
        {
            try
            {
                var edicaoUsuarioModel = new EdicaoUsuarioModel()
                {
                    Imagem = model.Imagem,
                    Nome = model.Nome,
                };

                var usuarioEdicao = await _usuarioServico.EditarUsuario(model.Email, edicaoUsuarioModel);
                if (usuarioEdicao.usuario == null)
                {
                    return BadRequest(new RetornoViewModel(null, false, usuarioEdicao.mensagem != null ? usuarioEdicao.mensagem : Registro.ErroEdicao));
                }

                usuarioEdicao.usuario.Senha = null;
                var usuario = new
                {
                    Usuario = usuarioEdicao.usuario,
                };
                return Ok(new RetornoViewModel(usuario, true, Registro.UsuarioEditado));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

        [HttpGet("consultar")]
        [Authorize]
        public async Task<IActionResult> Consultar(string email)
        {
            try
            {
                var usuario = await _usuarioServico.ObterUsuarioPorEmail(email);

                if (usuario == null)
                    return Ok(new RetornoViewModel(null, false, Registro.UsuarioInexistente));

                var usuarioConsulta = new
                {
                    Usuario = usuario,
                };

                return Ok(new RetornoViewModel(usuarioConsulta, true, Info.Sucesso));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }


        [HttpGet("obterImagem")]
        [Authorize]
        public async Task<IActionResult> ObterImagem(string email)
        {
            try
            {
                var usuario = await _usuarioServico.ObterUsuarioPorEmail(email);
                var imagem = Convert.FromBase64String(usuario.Imagem);
                return File(imagem, "image/png");
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

        [HttpPost("refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenViewModel model)
        {
            try
            {
                var tokenUsuario = TokenServico.ObterClaimsToken(model.TokenExpirado);
                var email = tokenUsuario?.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
                var usuario = await _usuarioServico.ObterUsuarioPorEmail(email);

                var novoToken = TokenServico.GerarToken(usuario);
                var novoRefreshToken = TokenServico.GerarRefreshToken();

                var atualizaRefreshToken = await _usuarioRefreshTokenServico.AtualizarRefreshToken(usuario.Id, model.RefreshToken, novoRefreshToken);
                if (atualizaRefreshToken == null)
                    return BadRequest(new RetornoViewModel(null, false, Info.ErroRefreshToken));

                var dadosRetorno = new
                {
                    Token = novoToken,
                    RefreshToken = novoRefreshToken,
                    DataExpiracao = TokenServico.ObterDataExpiracaoToken(novoToken),
                };
                return Ok(new RetornoViewModel(dadosRetorno, true, null));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

        [HttpGet("ativarConta")]
        public async Task<IActionResult> AtivarConta(string token, string? urlRedirecionamento)
        {
            try
            {
                var tokenUsuario = TokenServico.ObterClaimsToken(token);
                var email = tokenUsuario?.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
                var dataExpiracao = TokenServico.ObterDataExpiracaoToken(token);

                if (dataExpiracao < DateTime.Now)
                    return BadRequest(new RetornoViewModel(null, false, Registro.TokenExpirado));

                var ativacao = await _usuarioServico.AtivarUsuario(email); 

                if (ativacao && !string.IsNullOrEmpty(urlRedirecionamento))
                    return Redirect(urlRedirecionamento);

                if (ativacao && string.IsNullOrEmpty(urlRedirecionamento))
                    return Ok(new RetornoViewModel(null, true, Registro.ContaAtivada));
          
                return BadRequest(new RetornoViewModel(null, false, Registro.ErroAoAtivarConta));
            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

        [HttpPost("reenviarEmailAtivacaoConta")]
        [AllowAnonymous]
        public async Task<IActionResult> ReenviarEmailAtivacaoConta(EnvioEmailViewModel model)
        {
            try
            {
                var usuario = await _usuarioServico.ObterUsuarioPorEmail(model.Email);
                if (usuario == null)
                    return BadRequest(new RetornoViewModel(null, false, Registro.UsuarioInexistente));

                var token = TokenServico.GerarToken(usuario, 10);

                _emailServico.EnviarEmail("Autenticação de Usuários - Ativacão de conta", $"Olá {usuario.Nome} você solicitou uma nova confirmação para seu e-mail. " +
                                          $"Clique no link abaixo para validar sua conta <br> <br>" +
                                          $"<strong>Confirmação:/<strong> https://localhost:7053/api/usuario/ativarConta?token={token}&urlRedirecionamento={model.UrlRedirecionamento}", usuario.Email);

                return Ok(new RetornoViewModel(null, true, Registro.EmailEnviado));

            }
            catch (Exception)
            {
                return StatusCode(500, new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }

    }
}
