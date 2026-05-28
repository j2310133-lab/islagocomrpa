using Microsoft.AspNetCore.Mvc;

namespace ISLAGO_V3.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
