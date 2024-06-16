using AutenticacaoDeUsuarios.Data.Models;
using AutenticacaoDeUsuarios.MetodosDeExtensao;
using AutenticacaoDeUsuarios.Servicos.UsuarioApi;
using AutenticacaoDeUsuarios.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AutenticacaoDeUsuarios.Controllers
{
    [Authorize]
    public class DadosUsuarioController : BaseController
    {
        public DadosUsuarioController(UsuarioApiServico usuario, IMemoryCache cache): base(usuario, cache)
        {
        }
        public IActionResult Index()
        {
            return View(this.Usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Salvar(DadosViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new RetornoViewModel(null, false, "Dados inválidos"));
                }
                string? foto = null;
                if (model.Foto != null)
                     foto = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Foto, "") ?? null;
                var edicao = await _usuario.Editar(User.Identity?.Name, model.Nome, foto);
                if (edicao.usuario != null)
                {
                    _cache.Remove($"CacheUsuario_{User?.Identity?.Name}");
                    return Json(new RetornoViewModel(edicao.usuario, true, "Salvo com sucesso"));
                }

                return Json(new RetornoViewModel(null, false, edicao.mensagem));
            }
            catch (Exception)
            {
                return Json(new RetornoViewModel(null, false, "Erro ao salvar"));
                throw;
            }
        }
    }
}
