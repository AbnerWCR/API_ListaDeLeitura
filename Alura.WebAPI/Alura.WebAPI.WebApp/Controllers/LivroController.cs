using Alura.ListaLeitura.HttpClients;
using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        #region "Variaveis"

        private readonly LivroApiClient _api;

        #endregion

        #region "Construtor"

        public LivroController(LivroApiClient api)
        {
            _api = api;
        }

        #endregion

        #region "Metodos Publicos"

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novo(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                await _api.PostLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ImagemCapa(int id)
        {
            byte[] img = await _api.GetCapaLivroAsync(id);
            
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        //public Livro RecuperarLivro(int id)
        //{
        //    return _repo.Find(id);
        //}

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var model = await _api.GetLivroApiAsync(id);
            
            if (model == null)
            {
                return NotFound();
            }
            return View(model.ToUpload());
        }

        //public ActionResult<LivroUpload> DetalhesJson(int id)
        //{
        //    var livro = RecuperarLivro(id);

        //    if (livro == null)
        //    {
        //        return NotFound();
        //    }

        //    return livro.ToModel();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detalhes(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                //var livro = model.ToLivro();
                //if (model.Capa == null)
                //{
                //    livro.ImagemCapa = _repo.All() 
                //        .Where(l => l.Id == livro.Id)
                //        .Select(l => l.ImagemCapa)
                //        .FirstOrDefault();
                //}
                //_repo.Alterar(livro);

                await _api.PutLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int id)
        {
            var model = await _api.GetLivroApiAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            await _api.DeleteLivroAsync(id);
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}