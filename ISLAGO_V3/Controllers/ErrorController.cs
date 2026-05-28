using Microsoft.AspNetCore.Mvc;

namespace ISLAGO_V3.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            ViewBag.StatusCode = statusCode;

            return View();
        }
    }
}