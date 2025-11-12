using Microsoft.AspNetCore.Mvc;
using CapaEntidades;
using CapaWeb.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using CapaData.Interfaaces;

namespace CapaWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioRepositorio _repositorio;

        public AccesoController(IUsuarioRepositorio repositorio)
        {
            _repositorio = repositorio;
        }
        public IActionResult Login()
        {
            if (User.Identity is not null && User.Identity.IsAuthenticated)
            {
                string? rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                return rol switch
                {
                    "Administrador" => RedirectToAction("Index", "Usuario"),
                    "Cajero" => RedirectToAction("Index", "Producto"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Debe completar todos los campos.";
                return View();
            }

            Usuario? usuario_encontrado = await _repositorio.LoginNuevo(modelo.Correo, modelo.Clave);

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "Correo o clave incorrectos.";
                return View();
            }

            // Crear Claims para autenticación
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario_encontrado.Nombre} {usuario_encontrado.Apellido}"),
                new Claim(ClaimTypes.Role, usuario_encontrado.RolUsuario.Nombre!)
            ];

            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new() { AllowRefresh = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return usuario_encontrado.RolUsuario.Nombre switch
            {
                "Cajero" => RedirectToAction("Index", "Producto"),
                "Administrador" => RedirectToAction("Index", "Usuario"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public IActionResult Denegado()
        {
            return View();
        }

    }
}
