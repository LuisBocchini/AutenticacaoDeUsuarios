using AutenticacaoDeUsuarios.Models;
using AutenticacaoDeUsuarios.Servicos.UsuarioApi;
using AutenticacaoDeUsuarios.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutenticacaoDeUsuarios.Business.Mensagens;
using Microsoft.Extensions.Caching.Memory;

namespace AutenticacaoDeUsuarios.Controllers
{
    public class LoginController : BaseController
    {
        public LoginController(UsuarioApiServico usuario, IMemoryCache cache) : base(usuario, cache)
        {
      
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/Home/Index");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Autenticar(LoginViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                    return Json(new RetornoViewModel(null, false, "Email inválido"));
                if (string.IsNullOrEmpty(model.Senha))
                    return Json(new RetornoViewModel(null, false, "Senha inválida"));

                var autenticacao = await _usuario.Autenticar(model.Email, model.Senha);
                if(autenticacao.usuario == null)
                    return Json(new RetornoViewModel(null, false, autenticacao.mensagem));

                await GerarSessaoUsuario(autenticacao.usuario);
                return Json(new RetornoViewModel(null, true, null));
            }
            catch (Exception)
            {
                return Json(new RetornoViewModel(null, false, Info.ErroAutenticacao));
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CadastroViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                    return Json(new RetornoViewModel(null, false, "Email inválido"));
                if (string.IsNullOrEmpty(model.Senha))
                    return Json(new RetornoViewModel(null, false, "Senha inválida"));
                if (string.IsNullOrEmpty(model.Nome))
                    return Json(new RetornoViewModel(null, false, "Nome inválido"));

                var cadastro = await _usuario.Cadastrar(model.Nome,model.Email, model.Senha);
                if (cadastro.usuario == null)
                    return Json(new RetornoViewModel(null, false, cadastro.mensagem));

                return Json(new RetornoViewModel(null, true, cadastro.mensagem));
            }
            catch (Exception)
            {
                return Json(new RetornoViewModel(null, false, Info.ErroAutenticacao));
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> EnviarEmailRedefinicaoSenha(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return Json(new RetornoViewModel(null, false, Registro.EmailInvalido));
                
                var envioEmail = await _usuario.EnviarEmailRedefinicaoSenha(email);
                if (!envioEmail.sucesso)
                    return Json(new RetornoViewModel(null, false, envioEmail.mensagem));

                return Json(new RetornoViewModel(null, true, envioEmail.mensagem));
            }
            catch (Exception)
            {
                return Json(new RetornoViewModel(null, false, Info.ErroInterno));
                throw;
            }
        }
        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Login");
        }
        public async Task GerarSessaoUsuario(UsuarioAplicacao? usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("Nome", usuario?.Nome),
                new Claim("Token", usuario?.Token),
                new Claim("RefreshToken", usuario?.RefreshToken),
                new Claim("DataExpiracaoToken", usuario?.DataExpiracaoToken.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties { };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
