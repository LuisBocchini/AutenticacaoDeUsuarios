using AutenticacaoDeUsuarios.Servicos.UsuarioApi;
using AutenticacaoDeUsuarios.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AutenticacaoDeUsuarios.Controllers
{
    public class AlterarSenhaController : BaseController
    {
        public AlterarSenhaController(UsuarioApiServico usuario, IMemoryCache cache) : base(usuario, cache) { }
        public IActionResult Index(string? token, string? email)
        {
            if(string.IsNullOrWhiteSpace(token) || string.IsNullOrEmpty(email))
                ViewBag.PossuiSenhaAtual = true;
            else
                ViewBag.PossuiSenhaAtual = false;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Salvar(string? senhaAtual, string? novaSenha, string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                if (string.IsNullOrEmpty(senhaAtual) || string.IsNullOrEmpty(novaSenha))
                    return Json("Parâmetros inválidos");

                if (await _usuario.AtualizarSenha(senhaAtual, novaSenha, this.Usuario?.Email))
                    return Json(new RetornoViewModel(null, true, "Senha alterada"));
            }
            else
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(novaSenha))
                    return Json("Parâmetros inválidos");

                var redefinicaoSenha = await _usuario.RedefinirSenha(novaSenha, token);

                if (redefinicaoSenha.sucesso)
                    return Json(new RetornoViewModel(null, true, "Senha alterada"));
                else
                    return Json(new RetornoViewModel(null, false, redefinicaoSenha.mensagem));
            }
            return Json(new RetornoViewModel(null, false, "Erro ao alterar a senha"));
        }
    }
}
