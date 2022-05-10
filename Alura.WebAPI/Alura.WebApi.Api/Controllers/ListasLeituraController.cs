using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ListasLeituraController : ControllerBase
    {

        private readonly IRepository<Livro> _repo;

        public ListasLeituraController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        private Lista CriarLista(TipoListaLeitura tipoListaLeitura)
        {
            return new Lista
            {
                Tipo = tipoListaLeitura.ParaString(),
                Livros = _repo.All
                .Where(x => x.Lista == tipoListaLeitura)
                .Select(x => x.ToApi())
                .ToList(),
            };
        }

        [HttpGet]
        public IActionResult TodasListas()
        {
            Lista paraLer = CriarLista(TipoListaLeitura.ParaLer);
            Lista lendo = CriarLista(TipoListaLeitura.Lendo);
            Lista lidos = CriarLista(TipoListaLeitura.Lidos);

            var colecao = new List<Lista>() { paraLer, lidos, lendo };
            return Ok(colecao);
        }

        [HttpGet("{tipoLista}")]
        public IActionResult RecuperarTipoLista(TipoListaLeitura tipoLista)
        {
            var lista = CriarLista(tipoLista);

            if (lista == null)
            {
                return NotFound();
            }

            return Ok(lista);
        }
    }
}
