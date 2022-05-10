using Alura.ListaLeitura.HttpClients;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region "Variaveis"

        private readonly LivroApiClient _api;

        #endregion

        #region "Construtor"

        public HomeController(LivroApiClient api)
        {
            _api = api;
        }

        #endregion

        #region "Metodos Publicos"

        private async Task<IEnumerable<LivroApi>> ListaDoTipo(TipoListaLeitura tipo)
        {
            var lista =  await _api.GetListaLeituraAsync(tipo);

            return lista.Livros;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.User.Claims.First(user => user.Type == "Token").Value;

            var model = new HomeViewModel
            {
                ParaLer = await ListaDoTipo(TipoListaLeitura.ParaLer),
                Lendo   = await ListaDoTipo(TipoListaLeitura.Lendo),
                Lidos   = await ListaDoTipo(TipoListaLeitura.Lidos)
            };
            return View(model);
        }

        #endregion
    }
}