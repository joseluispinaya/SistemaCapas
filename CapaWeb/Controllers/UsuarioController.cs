using CapaData.Interfaaces;
using CapaEntidades;
using CapaWeb.Models;
using CapaWeb.Utilidades.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CapaWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IRolUsuarioRepositorio _repositorioRol;
        //private readonly IRolUsuarioRepositorio _repositorioRol;

        public UsuarioController(IUsuarioRepositorio repositorio, IRolUsuarioRepositorio repositorioRol)
        {
            _repositorio = repositorio;
            _repositorioRol = repositorioRol;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            List<RolUsuario> lista = await _repositorioRol.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            List<Usuario> lista = await _repositorio.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Usuario objeto)
        {
            GenericResponse<bool> gResponse = new();

            try
            {
                // Obtener la lista de usuarios desde el repositorio
                var listaUsuarios = await _repositorio.Lista();

                // Validar que el correo no esté repetido (ignorando mayúsculas/minúsculas)
                bool existeCorreo = listaUsuarios.Any(u => u.Correo.Equals(objeto.Correo, StringComparison.OrdinalIgnoreCase));

                if (existeCorreo)
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "El correo ya existe, ingrese otro.";
                    return StatusCode(StatusCodes.Status200OK, gResponse);
                    //return Ok(gResponse);
                }

                // Guardar si no existe
                bool respuesta = await _repositorio.Guardar(objeto);

                gResponse.Estado = respuesta;
                gResponse.Mensaje = respuesta
                    ? "Se registró correctamente."
                    : "Error al registrar, ingrese otro CI o intente más tarde.";
            }
            catch (Exception)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "Ocurrió un error inesperado.";
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
            //return Ok(gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Usuario objeto)
        {
            GenericResponse<bool> gResponse = new();

            try
            {
                // Obtener la lista de usuarios existentes
                List<Usuario> listaUsuarios = await _repositorio.Lista();

                // Validar que el correo NO esté usado por otro usuario
                bool correoExiste = listaUsuarios.Any(u =>
                    u.Correo.Equals(objeto.Correo, StringComparison.OrdinalIgnoreCase)
                    && u.IdUsuario != objeto.IdUsuario // excluir al mismo usuario que se está editando
                );

                if (correoExiste)
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "El correo ya está siendo usado por otro usuario.";
                    return StatusCode(StatusCodes.Status200OK, gResponse);
                    //return Ok(gResponse);
                }

                // LLamar al método Editar del repositorio
                bool respuesta = await _repositorio.Editar(objeto);

                gResponse.Estado = respuesta;
                gResponse.Mensaje = respuesta
                    ? "El usuario fue actualizado correctamente."
                    : "No se pudo actualizar el usuario, intente nuevamente.";
            }
            catch (Exception)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "Ocurrió un error inesperado.";
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
            //return Ok(gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> EditarNuevo([FromBody] UsuarioDTO objeto)
        {
            GenericResponse<bool> gResponse = new();

            try
            {
                // Obtener la lista de usuarios existentes
                List<Usuario> listaUsuarios = await _repositorio.Lista();

                // Validar correo duplicado (excluyendo al usuario actual)
                bool correoExiste = listaUsuarios.Any(u =>
                    u.Correo.Equals(objeto.Correo, StringComparison.OrdinalIgnoreCase)
                    && u.IdUsuario != objeto.IdUsuario
                );

                if (correoExiste)
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "El correo ya está siendo usado por otro usuario.";
                    return StatusCode(StatusCodes.Status200OK, gResponse);
                    //return Ok(gResponse);
                }

                // Mapeo: DTO → Usuario (el repositorio espera Usuario)
                Usuario usuario = new()
                {
                    IdUsuario = objeto.IdUsuario,
                    NroCi = objeto.NroCi,
                    Nombre = objeto.Nombre,
                    Apellido = objeto.Apellido,
                    Correo = objeto.Correo,
                    Clave = objeto.Clave,
                    RolUsuario = new RolUsuario { IdRolUsuario = objeto.IdRolUsuario }
                };

                // Llamar al repositorio
                bool respuesta = await _repositorio.Editar(usuario);

                gResponse.Estado = respuesta;
                gResponse.Mensaje = respuesta
                    ? "El usuario fue actualizado correctamente."
                    : "No se pudo actualizar el usuario.";
            }
            catch (Exception)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "Ocurrió un error inesperado.";
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
            //return Ok(gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarNuevo([FromBody] UsuarioDTO objeto)
        {
            GenericResponse<bool> gResponse = new();

            try
            {
                List<Usuario> listaUsuarios = await _repositorio.Lista();

                bool correoExiste = listaUsuarios.Any(u =>
                    u.Correo.Equals(objeto.Correo, StringComparison.OrdinalIgnoreCase));

                if (correoExiste)
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "El correo ya está siendo usado por otro usuario.";
                    return StatusCode(StatusCodes.Status200OK, gResponse);
                    //return Ok(gResponse);
                }

                Usuario usuario = new()
                {
                    NroCi = objeto.NroCi,
                    Nombre = objeto.Nombre,
                    Apellido = objeto.Apellido,
                    Correo = objeto.Correo,
                    Clave = objeto.Clave,
                    RolUsuario = new RolUsuario { IdRolUsuario = objeto.IdRolUsuario }
                };

                bool respuesta = await _repositorio.Guardar(usuario);

                gResponse.Estado = respuesta;
                gResponse.Mensaje = respuesta
                    ? "Se registró correctamente."
                    : "Error al registrar.";
            }
            catch (Exception)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "Ocurrió un error inesperado.";
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
            //return Ok(gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarOri([FromBody] Usuario objeto)
        {
            GenericResponse<bool> gResponse = new();

            try
            {
                bool respuesta = await _repositorio.Guardar(objeto);
                gResponse.Estado = respuesta;
                gResponse.Mensaje = respuesta
                    ? "Se registró correctamente."
                    : "Error al registrar, ingrese otro CI o intente más tarde.";

            }
            catch (Exception)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "Ocurrió un error inesperado.";
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

    }
}
