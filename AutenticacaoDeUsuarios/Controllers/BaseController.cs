using AutenticacaoDeUsuarios.Data.Models;
using AutenticacaoDeUsuarios.MetodosDeExtensao;
using AutenticacaoDeUsuarios.Models;
using AutenticacaoDeUsuarios.Servicos.UsuarioApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace AutenticacaoDeUsuarios.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UsuarioApiServico _usuario;
        protected readonly IMemoryCache _cache;

        public BaseController(UsuarioApiServico usuario, IMemoryCache cache)
        {
            _usuario = usuario;
            _cache = cache;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.usuario = this.Usuario;
        }
        public UsuarioAplicacao? Usuario
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    var usuarioCache = _cache.Get<UsuarioAplicacao>($"CacheUsuario_{User.Identity.Name}");
                    if (usuarioCache != null)
                        return usuarioCache;

                    var consultaUsuario = _usuario.Consultar(User.Identity?.Name);
                    var usuario = consultaUsuario.Result.usuario;

                    var configCache = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(20));
                    _cache.Set($"CacheUsuario_{User?.Identity?.Name}", usuario, configCache);

                    return usuario;
                }
                return null;
            }
        }
    }
}
