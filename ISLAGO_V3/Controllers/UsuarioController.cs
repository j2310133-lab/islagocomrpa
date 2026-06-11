using ISLAGO_V3.Datos;
using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Models.ViewModels;
using ISLAGO_V3.Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISLAGO_V3.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly DBContextISLAGO _context;

        public UsuarioController(
            IUsuarioService usuarioService,
            DBContextISLAGO context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}