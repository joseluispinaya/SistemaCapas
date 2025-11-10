using CapaData.Interfaaces;
using CapaEntidades;
using CapaWeb.Utilidades.Helpers;
using CapaWeb.Utilidades.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CapaWeb.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoRepositorio _repositorio;
        private readonly IImageHelper _imageHelper;

        public ProductoController(IProductoRepositorio repositorio, IImageHelper imageHelper)
        {
            _repositorio = repositorio;
            _imageHelper = imageHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Producto> lista = await _repositorio.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [HttpPost]
        public async Task<IActionResult> GuardarFoto([FromForm] IFormFile foto)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                if (foto != null)
                {
                    var path = await _imageHelper.UploadImageAsync(foto); // aquí guardas la imagen

                    gResponse.Estado = true;
                    gResponse.Mensaje = "Imagen guardada correctamente.";
                    gResponse.Objeto = path;    // ruta de la imagen guardada

                    return StatusCode(StatusCodes.Status200OK, gResponse);
                }
                else
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "No se envió ninguna imagen.";

                    return StatusCode(StatusCodes.Status400BadRequest, gResponse);
                }
            }
            catch (Exception)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "No se pudo guardar la imagen.";

                // OPCIONAL: si quieres registrar el error:
                // gResponse.Mensaje = ex.Message;

                return StatusCode(StatusCodes.Status500InternalServerError, gResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Guardar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            bool rpta;

            try
            {
                // Deserializar el JSON recibido desde el FormData
                Producto producto = JsonConvert.DeserializeObject<Producto>(modelo)!;

                // Si hay foto, subirla
                if (foto != null)
                {
                    producto.ImagenPro = await _imageHelper.UploadImageAsync(foto);
                }

                // Guardar en la BD
                rpta = await _repositorio.Guardar(producto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Guardar Producto: " + ex.Message);
                rpta = false;
            }

            return StatusCode(StatusCodes.Status200OK, new { data = rpta });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile? foto, [FromForm] string modelo)
        {
            bool rpta;

            try
            {
                // Deserializamos el JSON que viene desde el Fetch/FormData
                Producto producto = JsonConvert.DeserializeObject<Producto>(modelo)!;

                // Si viene una nueva imagen, la guardamos y reemplazamos la existente
                if (foto != null)
                {
                    producto.ImagenPro = await _imageHelper.UploadImageAsync(foto);
                }

                rpta = await _repositorio.Editar(producto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Editar Producto: " + ex.Message);
                rpta = false;
            }

            return StatusCode(StatusCodes.Status200OK, new { data = rpta });
        }

        [HttpPost]
        public async Task<IActionResult> GuardarOrigi([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            bool rpta;
            try
            {
                Producto producto = JsonConvert.DeserializeObject<Producto>(modelo)!;
                var path = string.Empty;
                if (foto != null)
                {
                    path = await _imageHelper.UploadImageAsync(foto);
                }

                var producAdd = new Producto
                {
                    IdProducto = producto.IdProducto,
                    ImagenPro = path,
                    Nombre = producto.Nombre
                    // demas propiedades
                };

                rpta = await _repositorio.Guardar(producAdd);
            }
            catch (Exception)
            {
                rpta = false;
            }
            return StatusCode(StatusCodes.Status200OK, new { data = rpta });
        }

    }
}
