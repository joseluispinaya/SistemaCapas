using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CapaWeb.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var nombreUsuario = string.Empty;
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            if (claimsPrincipal?.Identity?.IsAuthenticated == true)
            {
                // Busca directamente el claim de tipo "Name"
                nombreUsuario = claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            }
            ViewData["nombreUsuario"] = nombreUsuario;

            return View();
        }
    }
}
