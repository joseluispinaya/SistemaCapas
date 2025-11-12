using CapaData.Interfaaces;
using CapaEntidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CapaWeb.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepositorio _repositorio;

        public CategoriaController(ICategoriaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Categoria> lista = await _repositorio.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Categoria objeto)
        {
            bool respuesta = await _repositorio.Guardar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Categoria objeto)
        {
            bool respuesta = await _repositorio.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }


    }
}
