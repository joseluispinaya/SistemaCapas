using Microsoft.AspNetCore.Mvc;

namespace CapaWeb.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
