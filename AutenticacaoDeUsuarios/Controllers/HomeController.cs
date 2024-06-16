using AutenticacaoDeUsuarios.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using AutenticacaoDeUsuarios.Servicos.UsuarioApi;
using Microsoft.Extensions.Caching.Memory;

namespace AutenticacaoDeUsuarios.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UsuarioApiServico usuario, IMemoryCache cache) : base(usuario, cache)
        {
            _logger = logger;
        }
     
        public IActionResult Index()
        {
            return View(this.Usuario);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
